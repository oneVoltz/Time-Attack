using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 300;
    public int currentHealth;
    public Slider bossHealthSlider;
    public GameObject panelWinMessege;
    public TMP_Text text;
    public GameObject player;
    public PlayerHealth playerHealth;
    void Awake()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        panelWinMessege.SetActive(false);
    }
    void Start()
    {
        currentHealth = maxHealth;

    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            panelWinMessege.SetActive(true);
            text.text = ($"You win with this amount of Health {playerHealth.currentHealth}, try to win with more health next time");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseGame();
            Die();

        }

    }



    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {

        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }


    void Die()
    {
        currentHealth = 0;
        bossHealthSlider.value = currentHealth;
        Destroy(gameObject);

    }

    public void Update()
    {
        bossHealthSlider.value = currentHealth;
    }
}
