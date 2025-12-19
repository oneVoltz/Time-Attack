using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    public void StartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void SettingButton()
    {
        if (settingsPanel.activeInHierarchy)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            settingsPanel.SetActive(true);
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void Awake()
    {
        settingsPanel.SetActive(false);
    }
    public void Update()
    {
        int Scene = SceneManager.GetActiveScene().buildIndex;
        if (Scene == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
