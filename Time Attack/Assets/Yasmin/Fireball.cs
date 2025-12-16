using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 15;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
