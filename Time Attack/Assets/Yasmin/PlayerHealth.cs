using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public bool isPaused;
    public GameObject panelDieMessege;
    public TMP_Text text;
    public float timer=0;

    public void Awake()
    {
        panelDieMessege.SetActive(false);
        isPaused = false;
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
            Debug.Log("Player Dead");
            currentHealth = 0;
            healthSlider.value = currentHealth;
            panelDieMessege.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseGame();
        }

          
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resumed");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Update()
    {
       
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            currentHealth -= 1;
            timer -= 1f;
        }

       text.text = currentHealth.ToString();
        healthSlider.value = currentHealth;
    }
}
