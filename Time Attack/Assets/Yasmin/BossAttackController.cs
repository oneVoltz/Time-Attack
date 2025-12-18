using UnityEngine;
using System.Collections;
using UnityEngine.Animations;

public class BossAttackController : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb;

    public float attackCooldown = 1.5f;

    [Header("Slam")]
    public float slamRadius = 5f;
    public int slamDamage = 20;

    [Header("Fireballs")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballForce = 20f;

    [Header("Fireball Spread")]
    public int spreadCount = 5;
    public float spreadAngle = 30f;

    [Header("Dash")]
    public float dashForce = 25f;
    public float dashTime = 0.3f;

    [Header("Shockwave")]
    public float shockwaveRadius = 8f;
    public int shockwaveDamage = 25;
    public float shockwaveDelay = 0.6f;

    [Header("Spin Charge")]
    public float spinSpeed = 720f;
    public float chargeSpeed = 18f;
    public float chargeTime = 1.2f;
    public int chargeDamage = 30;

    BossJumpAroundPlayer jumpAroundPlayer;
    public bool canAttack = true;
    int attackValue;
    int actionHappening;

    void Start()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    public void TryAttack()
    {
        if (!canAttack) return;
      
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
       
        int attack = GetRandomAttack();
        switch (attack)
        {
            case 0:
                jumpAroundPlayer.canJump = false;
                yield return Dash();
                jumpAroundPlayer.canJump = true;
                break;
            case 1:

                yield return Fireballs();
                break;
            //case 2: yield return Chill(); break;
            //case 3: yield return FireballSpread(); break;
            //case 4: yield return Shockwave(); break;
            
        }

        yield return new WaitForSeconds(attackCooldown);
        
    }

    int GetRandomAttack()
    {
        int attack = Random.Range(0, 2);
        return attack;
        
    }
    public IEnumerator Fireballs()
    {
     
        for (int i = 0; i < 3; i++)
        {
            Vector3 dir = (player.position - firePoint.position).normalized;
            SpawnFireball(dir);
            yield return new WaitForSeconds(0.25f);
        }
      

    }
    public IEnumerator Dash()
    {
      
        //dash cooldown
        transform.localScale = new Vector3(1,0.5f,1);
        yield return new WaitForSeconds(1f);
        transform.localScale = new Vector3(1, 1, 1);
        Vector3 dir = (player.position - transform.position).normalized;
        float t = 0;

        while (t < dashTime)
        {
            rb.linearVelocity = dir * dashForce;
            t += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
       
    }
  
    /*IEnumerator FireballSpread()
    {
        float startAngle = -spreadAngle * 0.5f;
        float step = spreadAngle / (spreadCount - 1);

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + step * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
           SpawnFireball(dir);
        }

        yield return new WaitForSeconds(0.4f);
    }*/



    /* IEnumerator Shockwave()
     {
         yield return new WaitForSeconds(shockwaveDelay);

         foreach (Collider hit in Physics.OverlapSphere(transform.position, shockwaveRadius))
         {
             if (hit.CompareTag("Player"))
                 hit.GetComponent<PlayerHealth>()?.TakeDamage(shockwaveDamage);
         }
     }
    */
    void SpawnFireball(Vector3 dir)
    {
        GameObject fb = Instantiate(
            fireballPrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        fb.GetComponent<Rigidbody>()
          .AddForce(dir * fireballForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.collider.GetComponent<PlayerHealth>()?.TakeDamage(chargeDamage);
    }
}
