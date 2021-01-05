using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private GameManager m_GameManager;

    public void ResumeSelected()
    {
        Debug.Log("Playing");
        m_GameManager.DisableScreen("Pause");
        m_GameManager.m_CurrentScreen = ScreenType.Playing;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void RetrySelected()
    {
        Debug.Log("Retry");
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void MenuSelected()
    {
        Debug.Log("Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
