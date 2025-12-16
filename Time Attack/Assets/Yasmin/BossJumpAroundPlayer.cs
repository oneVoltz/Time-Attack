using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BossJumpAroundPlayer : MonoBehaviour
{
    public Transform player;
    public float jumpForce = 12f;
    public float sideForce = 8f;
    public float jumpRadius = 6f;
    public float jumpCooldown = 2f;

    public LayerMask groundLayer;
    public float groundCheckDistance = 1.2f;

    Rigidbody rb;
    BossAttackController attackController;
    bool canJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        attackController = GetComponent<BossAttackController>();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        FacePlayer();

        if (canJump && IsGrounded())
        {
            attackController.TryAttack();
            StartCoroutine(Jump());
        }
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 5f
            );
    }

    IEnumerator Jump()
    {
        canJump = false;

        float angle = Random.Range(0f, 360f);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * jumpRadius;
        Vector3 target = player.position + offset;
        Vector3 direction = (target - transform.position).normalized;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * sideForce + Vector3.up * jumpForce, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
