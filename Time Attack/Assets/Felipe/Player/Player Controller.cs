using System.Collections;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveNew : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotationX = 0f;
    private bool isGrounded;
    private bool isAttacking;

    [Header("Raycast")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.2f;

    [Header("Bullets")]
    public GameObject bulletsPrefab;
    public Transform bulletsPoint;
    public float bulletForce = 20f;
    public float shootCoolDown = 2;
    public float timeGunRecover;
    public Animator shootAnimation;


    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += _ => lookInput = Vector2.zero;

        inputActions.Player.Attack.performed += ctx => isAttacking = true;
        inputActions.Player.Attack.canceled += _ => isAttacking = false;

        inputActions.Player.Jump.performed += ctx =>
        {
            if (IsGrounded())
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        };
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse look
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime * 100f;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime * 100f;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
      
    }
    void SpawnBullets(Vector3 dir)
    {
        GameObject fb = Instantiate(
            bulletsPrefab,
            bulletsPoint.position,
            bulletsPoint.transform.rotation
        );
        
        fb.GetComponent<Rigidbody>()
          .AddForce(bulletsPoint.transform.forward * bulletForce, ForceMode.Impulse);
    }
   
    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 targetVelocity = move * moveSpeed;
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        //Attack
      
        timeGunRecover += Time.deltaTime * 2;
        if (isAttacking && timeGunRecover > shootCoolDown)
        {
            
            shootAnimation.SetBool("Shot", true);
           
            Vector3 dir = bulletsPoint.position;
            SpawnBullets(dir);
            timeGunRecover = 0;

        }
        else
        {
            shootAnimation.SetBool("Shot", false);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

   
}