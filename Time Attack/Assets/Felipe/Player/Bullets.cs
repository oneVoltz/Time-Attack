using UnityEngine;

public class Bullets : MonoBehaviour
{
    public int damage = 15;
    public float lifeTime = 15f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Boss"))
        {
            collision.collider.GetComponent<BossHealth>()?.TakeDamage(damage);
        } 
            Destroy(gameObject);
    }
}