using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    int maxHealth = 100;
    int currentHealth;
    void Awake()
    {
        currentHealth = maxHealth;
    }
    void Damaged(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) Destroy(gameObject);
    }
}
