using UnityEngine;
using UnityEngine.UI;

public class Civilian : MonoBehaviour, IDamageable
{

    [SerializeField] private float m_Health;
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private GameObject m_CivilianDeath1;
    [SerializeField] private GameObject m_CivilianDeath2;
    [SerializeField] private Collider m_BoxCollider;

    private float m_MaxHealth;
    private bool m_Dying = false;
    private float m_DeadTimer = 0.0f;
    private bool m_DeathSoundPlayed = false;
    private Animator m_Animator;
    private AudioManager m_AudioManager;
    private TextManager m_TextManager;
    private GameManager m_GameManager;
    private GameObject m_Canvas;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioManager = FindObjectOfType<AudioManager>();
        m_TextManager = FindObjectOfType<TextManager>();
        m_GameManager = FindObjectOfType<GameManager>();
        m_Canvas = GameObject.FindGameObjectWithTag("Canvas");
        m_MaxHealth = m_Health;
    }

    private void Update()
    {
        if (m_Dying)
        {
            if (m_DeadTimer > 15f)
            {
                Destroy(gameObject);
            }
            else
                m_DeadTimer += Time.deltaTime;
        }
    }

    public void TakeDamage(float amount, string type)
    {
        if (m_Canvas.transform.Find("Don't hit the civilians!") == null)
        {
            m_TextManager.ShowTextCanvas(new Vector2(0f, 600f), "Don't hit the civilians!", Color.red, new Vector2(700f, 200f));
        }

        if (!m_AudioManager.IsPlayingSound("Warning"))
            m_AudioManager.PlaySound("Warning");

        m_Health -= amount;

        if (m_Health <= 0f)
        {
            m_HealthBar.fillAmount = 0f;
        }
        else
        {
            m_HealthBar.fillAmount = m_Health / m_MaxHealth;
        }
        SetHealthBarColour();

        if (m_Health <= 0f)
        {
            Die(type);
        }
    }

    private void SetHealthBarColour()
    {
        float healthPercent = m_Health / m_MaxHealth;
        if (healthPercent >= 0.75f)
        {
            m_HealthBar.color = Color.green;
        }
        if (healthPercent < 0.75f && healthPercent > 0.35f)
        {
            m_HealthBar.color = Color.yellow;
        }
        if (healthPercent <= 0.35f)
        {
            m_HealthBar.color = Color.red;
        }
    }

    private void Die(string type)
    {
        if (!m_DeathSoundPlayed)
        {
            Vector3 position = transform.position;
            position.y += 4f;

            if(type != "Grenade")
            {
                if (type == "Head")
                {
                    Transform head = GetHead();
                    head.localScale = new Vector3(0f, 0f, 0f);
                    GameObject deathEffect = Instantiate(m_CivilianDeath1, position, Quaternion.LookRotation(transform.up));
                    Destroy(deathEffect.gameObject, 2f);

                    m_AudioManager.PlaySound("HeadPop");
                }
                SetDying(true);
                m_Animator.SetTrigger("DeathTrigger");
            }
            else
            {
                GameObject deathEffect = Instantiate(m_CivilianDeath2, position, Quaternion.LookRotation(transform.up));
                Destroy(deathEffect.gameObject, 2f);
                Destroy(gameObject);
            }

            int num = Random.Range(0, 2);
            if (num == 0)
            {
                m_AudioManager.RandomizePitchSound("EnemyDeath1");
                m_AudioManager.PlaySound("EnemyDeath1");
            }

            else
            {
                m_AudioManager.RandomizePitchSound("EnemyDeath2");
                m_AudioManager.PlaySound("EnemyDeath2");
            }

            GetComponent<Collider>().enabled = false;
            m_BoxCollider.enabled = true;
            transform.Find("Head").GetComponent<Collider>().enabled = false;
            GetComponent<AIMovement>().enabled = false;
            m_HealthBar.transform.parent.gameObject.SetActive(false);

            Transform cameraRot = Camera.main.transform;

            m_GameManager.ChangeTotalMoney(-100);
            m_TextManager.ShowTextGame(transform.position, Quaternion.LookRotation(cameraRot.forward), "-100", Color.red);
            m_DeathSoundPlayed = true;
        }

    }

    private void SetDying(bool decision)
    {
        m_Dying = decision;
    }

    public bool GetDying()
    {
        return m_Dying;
    }

    public float GetHealth()
    {
        return m_Health;
    }

    private Transform GetHead()
    {
        return transform.Find("LowManSkeleton").transform.Find("LowManHips").transform.Find("LowManSpine").transform.Find("LowManSpine1").transform.Find("LowManSpine2").transform.Find("LowManNeck").transform.Find("LowManHead").transform;
    }
}
