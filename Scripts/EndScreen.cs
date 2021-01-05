using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private TMP_Text m_SecondsText;
    [SerializeField] private TMP_Text m_AccuracyText;
    [SerializeField] private GameManager m_GameManager;

    private void OnEnable()
    {
        Time.timeScale = 0.0f;
        string tempPoints = m_GameManager.m_Score.ToString() + " points";
        m_ScoreText.text = tempPoints;
        string tempSeconds = m_GameManager.m_Time.ToString() + " seconds";
        m_SecondsText.text = tempSeconds;
        string tempAccuracy = m_GameManager.m_Accuracy.ToString() + "% accuracy";
        m_AccuracyText.text = tempAccuracy;
    }

    public void RetrySelected()
    {
        Debug.Log("Playing");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void LeaderboardSelected()
    {
        Debug.Log("Leaderboard");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(2);
    }

    public void MenuSelected()
    {
        Debug.Log("Menu");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
