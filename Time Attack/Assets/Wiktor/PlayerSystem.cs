using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    int maxHealth = 100;
    int currentHealth;
    void Awake()
    {
        currentHealth = maxHealth;
    }
    public void Damaged(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Took " + dmg + " damage");
        if (currentHealth < 0) Destroy(gameObject);
    }
}
