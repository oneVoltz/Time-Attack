using JetBrains.Annotations;
using UnityEngine;

public class Movement : MonoBehaviour
{
    CharacterController charControl;

    InputSystemActions inputSystemActions;
    float grav = -9.81f, sprintSpeed;
    Vector2 movement, camDir;
    Vector3 playerVelocity;
    GameObject cam;
    bool isGrounded, isSprinting;

    [Header("Player Settings")]
    public float speed = 2f, jumpHeight = 1.5f, camSens;
    void Awake()
    {
        cam = GameObject.Find("Main Camera");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        charControl = gameObject.AddComponent<CharacterController>();

        inputSystemActions = new InputSystemActions();

        inputSystemActions.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Move.canceled += _ => movement = Vector2.zero;

        inputSystemActions.Player.Look.performed += ctx => camDir = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Look.canceled += _ => camDir = Vector2.zero;

        inputSystemActions.Player.Sprint.performed += _ => speed = sprintSpeed;
        inputSystemActions.Player.Sprint.canceled += _ => speed = 2f;

        inputSystemActions.Player.Jump.started += _ =>
        {
            if (isGrounded)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * grav);
            }
        };
    }
    private void OnEnable()
    {
        inputSystemActions.Enable();
    }
    private void OnDisable()
    {
        inputSystemActions.Disable();
    }
    void Update()
    {
        float rotY = camDir.y * camSens;
        float rotX = camDir.x * camSens;

        transform.Rotate(0, rotX, 0);

        Vector3 camRot = cam.transform.eulerAngles;
        if (camRot.y > 270) camRot.y -= 360;
        camRot.x = Mathf.Clamp(camRot.x, -70, 70);
        cam.transform.Rotate(-rotY, 0, 0);
    }
    void FixedUpdate()
    {
        isGrounded = charControl.isGrounded;
        if (isGrounded)
        {
            playerVelocity.y = grav;
        }
        else
        {
            playerVelocity.y += grav * Time.fixedDeltaTime;
        }

        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        Vector3 lastMove = (move * speed) + (playerVelocity.y * Vector3.up);
        charControl.Move(lastMove * Time.fixedDeltaTime);
    }
}
