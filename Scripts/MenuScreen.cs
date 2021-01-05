using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private GameObject m_ControlsScreen;

    public void PlaySelected()
    {
        Debug.Log("Playing");
        SceneManager.LoadScene(1);
    }

    public void ControlsSelected()
    {
        Debug.Log("Controls");
        m_ControlsScreen.SetActive(true);
    }

    public void LeaderboardSelected()
    {
        Debug.Log("Leaderboard");
        SceneManager.LoadScene(2);
    }

    public void ExitSelected()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
