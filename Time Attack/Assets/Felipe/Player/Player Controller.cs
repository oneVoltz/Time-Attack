using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed = 6f;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode grappleKey = KeyCode.Mouse0;
    public KeyCode dashKey = KeyCode.Q;
    public KeyCode slideKey = KeyCode.E; // Added slide key

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        grappling,
        dashing,
        climbing,
        sliding
    }

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;
    public float regenDelay = 2f;
    private float regenTimer;

    [Header("UI")]
    public Image staminaBar;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI speedText;

    [Header("Grapple")]
    public float grappleRange = 50f;
    public float grappleSpeed = 10f;
    public LayerMask grappleableSurface;
    private bool isGrappling;
    private Vector3 grapplePoint;
    private LineRenderer lineRenderer;

    [Header("Dash")]
    public float dashForce = 30f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    private bool canDash = true;
    private bool isDashing;
    private float dashTimer;

    [Header("Climbing")]
    public float climbSpeed = 4f;
    public float climbCheckDistance = 1f;
    private bool isClimbing;
    private bool nearClimbable;
    private RaycastHit climbHit;
    public LayerMask climbableLayer;

    [Header("Sliding")]
    public float slideSpeed = 15f;
    public float slideDuration = 0.5f;
    private bool isSliding = false;
    private float slideTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        startYScale = transform.localScale.y;

        stamina = maxStamina;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        HandleStamina();

        if (staminaBar != null)
            staminaBar.fillAmount = stamina / maxStamina;

        if (stateText != null)
            stateText.text = $"State: {state}";

        if (speedText != null)
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float currentSpeed = flatVelocity.magnitude;
            speedText.text = $"Speed: {currentSpeed:F1}";
        }

        rb.linearDamping = grounded ? groundDrag : 0;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
        }

        CheckForClimbableWall();
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (isGrappling)
            GrappleMovement();
        if (isClimbing)
            Climb();

        HandleSliding();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        if (Input.GetKeyDown(grappleKey) && !isGrappling)
        {
            ShootGrapple();
        }
        else if (Input.GetKeyDown(grappleKey) && isGrappling)
        {
            StopGrapple();
        }

        if (Input.GetKeyDown(dashKey) && canDash && stamina > 0f)
        {
            StartCoroutine(PerformDash());
        }


        if (Input.GetKeyDown(slideKey) && state == MovementState.sprinting && !isSliding && grounded)
        {
            StartSlide();
        }


        if (nearClimbable)
        {
            if (Input.GetKey(KeyCode.W))
            {
                isClimbing = true;
            }
            else
            {
                isClimbing = false;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
            }

            if (Input.GetKeyDown(jumpKey))
            {
                isClimbing = false;
                rb.useGravity = true;

                Vector3 jumpDir = (transform.position - climbHit.point).normalized + transform.up;
                jumpDir.Normalize();

                rb.linearVelocity = Vector3.zero;
                rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            isClimbing = false;
            rb.useGravity = true;
        }
    }

    private void StateHandler()
    {
        if (isClimbing)
        {
            state = MovementState.climbing;
        }
        else if (isGrappling)
        {
            state = MovementState.grappling;
        }
        else if (isDashing)
        {
            state = MovementState.dashing;
        }
        else if (isSliding)
        {
            state = MovementState.sliding;
            moveSpeed = slideSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey) && stamina > 0f && verticalInput > 0)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        if (isGrappling || isDashing || isClimbing || isSliding) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void HandleStamina()
    {
        bool isSprinting = Input.GetKey(sprintKey) && verticalInput > 0 && grounded && !Input.GetKey(crouchKey);

        if (isSprinting && stamina > 0f)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            regenTimer = 0f;

            if (stamina <= 0f)
                stamina = 0f;
        }
        else if (!isDashing)
        {
            if (stamina < maxStamina)
            {
                regenTimer += Time.deltaTime;
                if (regenTimer >= regenDelay)
                {
                    stamina += staminaRegenRate * Time.deltaTime;
                    stamina = Mathf.Min(stamina, maxStamina);
                }
            }
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        dashTimer = dashDuration;

        stamina = 0f;
        regenTimer = 0f;

        Vector3 dashDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (dashDirection == Vector3.zero)
            dashDirection = orientation.forward;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dashDirection.normalized * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void ShootGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleRange, grappleableSurface))
        {
            grapplePoint = hit.point;
            isGrappling = true;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    private void GrappleMovement()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        rb.AddForce(direction * grappleSpeed, ForceMode.VelocityChange);

        if (Vector3.Distance(transform.position, grapplePoint) < 1f)
        {
            StopGrapple();
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.enabled = false;
    }

    private void CheckForClimbableWall()
    {
        nearClimbable = Physics.Raycast(transform.position, transform.forward, out climbHit, climbCheckDistance, climbableLayer);
    }

    private void Climb()
    {
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed, rb.linearVelocity.z);
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        stamina -= 10f; // sliding stamina cost
        if (stamina < 0) stamina = 0;

        // Optional: crouch player during slide
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
    }

    private void HandleSliding()
    {
        if (isSliding)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Keep vertical velocity zero to stay grounded
            rb.AddForce(orientation.forward * slideSpeed, ForceMode.VelocityChange);

            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;

                // Reset player scale after slide
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            }
        }
    }
}