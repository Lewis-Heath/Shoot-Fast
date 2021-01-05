using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameInputScreen : MonoBehaviour
{
    [SerializeField] private string m_PlayerInputName;
    [SerializeField] private GameObject m_InputField;
    [SerializeField] private GameManager m_GameManager;

    public void StoreName()
    {
        m_PlayerInputName = m_InputField.GetComponent<TMP_Text>().text;

        if(m_PlayerInputName.Length > 1)
        {
            m_GameManager.m_PlayerName = m_PlayerInputName;
            m_GameManager.DisableScreen("NameScreen");
            m_GameManager.EnableScreen("EndScreen");
            m_GameManager.m_CurrentScreen = ScreenType.EndScreen;
            m_GameManager.UpdateLeaderboard();
        }
    }
}
