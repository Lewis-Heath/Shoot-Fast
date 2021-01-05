using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject m_Door;
    [SerializeField] private GameObject m_Player;
    private AudioManager m_AudioManager;
    private Vector3 m_UpPosition;
    private Vector3 m_DownPosition;
    bool m_TriggerPressed = false;

    private void Start()
    {
        m_DownPosition = transform.position;
        m_UpPosition = new Vector3(0f, 2.0f, 0f);
        m_AudioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if((m_Player.transform.position - transform.position).magnitude < 6.5f && Input.GetKeyDown(KeyCode.E))
        {
            m_TriggerPressed = true;
            m_AudioManager.PlaySound("DoorOpening");
        }

        if (m_Door.transform.position.y < m_DownPosition.y + 6f)
        {
            if (m_TriggerPressed)
            {
                m_Door.transform.Translate(m_UpPosition * Time.deltaTime * 2.5f);
            }
        }
    }


}
