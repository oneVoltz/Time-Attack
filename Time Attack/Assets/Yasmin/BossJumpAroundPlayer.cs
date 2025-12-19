using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class BossJumpAroundPlayer : MonoBehaviour
{
    public Transform player;
    public float jumpForce = 12f;
    public float sideForce = 8f;
    public float jumpRadius = 6f;
    private float jumpCooldown = 3f;

    public LayerMask groundLayer;
    private float groundCheckDistance = 0.1f;

    public int jumpOrAttack;
    Rigidbody rb;
    BossAttackController attackController;
    public bool canJump = true;
    public BossHealth bossHealth;

  

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        attackController = GetComponent<BossAttackController>();
        jumpCooldown = 3f;
        attackController.attackCooldown = 1.5f;
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        FacePlayer();
        if (canJump && IsGrounded() == true)
        {
         
                attackController.TryAttack();
           
                StartCoroutine(Jump());
            
              



        }
        if (bossHealth.currentHealth<150)
        {
            jumpCooldown = 1.5f;
            attackController.attackCooldown = 0.5f;
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
        Vector3 footPos = transform.position + Vector3.down * 0.95f;
        return Physics.Raycast(footPos, Vector3.down, groundCheckDistance, groundLayer);
    }
}
