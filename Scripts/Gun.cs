using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour, IFireable
{
    [SerializeField] private GunType m_GunType;
    [SerializeField] private float m_FireRate;
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_Range;
    [SerializeField] private Vector2[] m_SpreadAngles;
    [SerializeField] private GameObject m_MuzzleFlash;
    [SerializeField] private GameObject m_BulletHole;
    [SerializeField] private GameObject m_EnemyHitEffect;
    [SerializeField] private GameObject m_CivilianHitEffect;
    [SerializeField] private GameObject m_WallHitEffect;
    [SerializeField] private GameObject m_ObstacleHitEffect;
    [SerializeField] private int m_MagazineSize;
    [SerializeField] private float m_RecoilSeverity;
    [SerializeField] private float m_MaxRecoil;
    [SerializeField] private float m_ReloadTime;
    [SerializeField] private GameObject m_HitMarker;
    [SerializeField] private float m_KnockbackForce;

    private bool m_Reloading = false;
    private float m_CurrentReloadingTime = 0.0f;
    private float m_Recoil = 0.0f;
    private float m_TimeSinceLastPressed;

    private GameObject m_AttatchPoint;

    private Animator m_Animator;

    private int m_BulletsLeft = 0;
    private GameObject m_EndOfBarrel;
    private GameObject m_TempMuzzleFlash;
    private int m_BulletHoleCount;
    private Text m_AmmoCountText; 

    private float m_TimeToFire;
    private bool m_Firing;
    private int m_BurstCount;

    private GameObject m_TextManagerGO;
    private TextManager m_TextManager;

    private AudioManager m_AudioManager;
    private GameObject m_Canvas;
    private GameManager m_GameManager;

    private GameObject m_HitMarkerGO;

    private float m_RecoilDelay;
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_BulletsLeft = m_MagazineSize;

        m_AttatchPoint = this.transform.parent.gameObject;

        m_EndOfBarrel = GetChildWithName(gameObject, "EndOfBarrel");
        m_BurstCount = 0;

        GameObject temp = GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD").gameObject;
        m_AmmoCountText = temp.transform.Find("AmmoCount").gameObject.GetComponent<Text>();

        string temp2 = m_MagazineSize + " / " + m_MagazineSize;
        m_AmmoCountText.text = temp2;

        m_TextManagerGO = GameObject.FindGameObjectWithTag("TextManager");
        m_TextManager = m_TextManagerGO.GetComponent<TextManager>();

        m_AudioManager = FindObjectOfType<AudioManager>();

        m_Canvas = GameObject.FindGameObjectWithTag("Canvas");

        m_GameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        m_AttatchPoint.GetComponentInParent<MouseMovement>().SetRecoil(m_Recoil);

        if (!m_Reloading)
        {
            if (m_GunType == GunType.AK47)
            {
                if (m_BulletsLeft == 0)
                {
                    m_Recoil = 0.0f;
                }
            }

            if (m_GunType == GunType.M107)
            {
                if (m_RecoilDelay > 0.5f)
                {
                    m_Recoil = 0.0f;
                }
                else
                {
                    m_RecoilDelay += Time.deltaTime;
                }
            }

            if (m_Firing)
            {
                m_TimeSinceLastPressed = 0f;
                if (CanFire())
                {
                    Fire();
                    m_TimeToFire = 60f / m_FireRate;
                }
            }
            if(!CanFire())
            {
                m_TimeToFire -= Time.deltaTime;
            }
            else if(!m_Firing && m_BurstCount != 0)
            {
                m_Recoil = 0.0f;
                m_TimeSinceLastPressed += Time.deltaTime;
                if(m_TimeSinceLastPressed > 0.25f)
                {
                    m_BurstCount = 0;
                }   
            }
            if(m_BulletsLeft == 0 && Input.GetButtonDown("Fire1"))
            {
                if (m_Canvas.transform.Find("Out of bullets, reload!") == null)
                {
                    m_TextManager.ShowTextCanvas(new Vector2(-450f, -715f), "Out of bullets, reload!", Color.red, new Vector2(600f, 200f));
                }

                if (!m_AudioManager.IsPlayingSound("NoBullets"))
                    m_AudioManager.PlaySound("NoBullets");
            }
        }
        else
        {
            m_Recoil = 0.0f;

            if (m_CurrentReloadingTime >= m_ReloadTime)
            {
                string temp = m_MagazineSize + " / " + m_MagazineSize;
                m_AmmoCountText.text = temp;
                if (m_Canvas.transform.Find("Out of bullets, reload!") != null)
                {
                    Destroy(m_Canvas.transform.Find("Out of bullets, reload!").gameObject);
                }
                m_TimeToFire = 0f;
                m_BurstCount = 0;
                m_BulletsLeft = m_MagazineSize;
                m_CurrentReloadingTime = 0f;
                m_Reloading = false;
                m_Animator.SetBool("Reloading", false);
                GetComponent<WeaponSway>().SetActive(true);
                if (m_Firing)
                {
                    StartFiring();
                }
            }
            else
            {
                m_CurrentReloadingTime += Time.deltaTime;
            }
        }
    }

    private void Fire()
    {
        if (m_BulletsLeft >= 1)
        {
            if (m_GunType == GunType.M107)
            {
                m_RecoilDelay = 0.0f;

                string shootName = m_GunType + "Shooting";
                m_AudioManager.StopSound(shootName);
                m_AudioManager.PlaySound(shootName);

                if(m_TempMuzzleFlash != null)
                {
                    Destroy(m_TempMuzzleFlash.gameObject);
                }
                m_TempMuzzleFlash = (GameObject)Instantiate(m_MuzzleFlash, m_EndOfBarrel.transform);
                Destroy(m_TempMuzzleFlash.gameObject, 0.25f);

            }

            //Magazine logic
            m_BulletsLeft--;
            string temp = m_BulletsLeft + " / " + m_MagazineSize;
            m_AmmoCountText.text = temp;

            //Recoil logic
            if(m_GunType == GunType.AK47)
            {
                m_Recoil *= m_RecoilSeverity;
                if (m_Recoil > m_MaxRecoil)
                {
                    m_Recoil = m_MaxRecoil;
                }
            }
            
            if(m_GunType == GunType.M107)
            {
                m_Recoil = m_MaxRecoil;
            }

            //Spread logic
            Vector2 spread = m_SpreadAngles[Mathf.Min(m_BurstCount, (m_SpreadAngles.Length - 1))];
            Transform camera = Camera.main.transform;
            Vector3 startPosition = camera.position;
            Vector3 direction = camera.forward;
            Vector3 offset = new Vector3(Mathf.Tan(spread.x), Mathf.Tan(spread.y), 0);
            direction += camera.InverseTransformDirection(offset);
            direction.Normalize();

            //Shooting logic
            RaycastHit hit;
            if (Physics.Raycast(startPosition, direction, out hit, m_Range))
            {
                if (hit.transform.tag != "Enemy" && hit.transform.tag != "Civilian")
                {
                    //Bullet holes logic
                    GameObject[] activeBulletHoles = GameObject.FindGameObjectsWithTag("BulletHole");
                    m_BulletHoleCount = activeBulletHoles.Length;
                    if (m_BulletHoleCount > 15)
                    {
                        Destroy(activeBulletHoles[0].gameObject);
                    }

                    if(hit.transform.tag == "Level")
                    {
                        GameObject wallHit = Instantiate(m_WallHitEffect, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                        Destroy(wallHit.gameObject, 2.5f);
                    }
                    if (hit.transform.tag == "Obstacle")
                    {
                        if(hit.rigidbody != null)
                            hit.rigidbody.AddExplosionForce(m_KnockbackForce, hit.point, 1f);
                        GameObject wallHit = Instantiate(m_ObstacleHitEffect, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                        Destroy(wallHit.gameObject, 2.5f);
                    }

                    GameObject bulletHole = Instantiate(m_BulletHole, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(-hit.normal));
                    bulletHole.transform.SetParent(hit.transform);
                    Destroy(bulletHole.gameObject, 5f);

                    if (!m_AudioManager.IsPlayingSound("BulletHit"))
                        m_AudioManager.PlaySound("BulletHit");

                    m_GameManager.IncreaseBulletMiss();
                }
                else
                {
                    float damage = m_Damage;
                    float distanceToPerson = Vector3.Distance(transform.position, hit.transform.position);
                    float dec = distanceToPerson / m_Range;
                    damage *= 1 - dec;

                    string personType = "";

                    if (hit.transform.tag == "Enemy")
                    {
                        GameObject enemyHit = Instantiate(m_EnemyHitEffect, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                        //enemyHit.transform.SetParent(hit.transform);
                        Destroy(enemyHit.gameObject, 2.5f);
                        personType = "Enemy";
                        
                    }

                    else if (hit.transform.tag == "Civilian")
                    {
                        GameObject enemyHit = Instantiate(m_CivilianHitEffect, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                        //enemyHit.transform.SetParent(hit.transform);
                        Destroy(enemyHit.gameObject, 2.5f);
                        personType = "Civilian";
                    }

                    if (hit.collider.name == "Head")
                    {
                        damage *= 2;
                        HitMarker(damage, hit, personType);
                        hit.collider.GetComponentInParent<IDamageable>().TakeDamage(damage, "Head");
                    }

                    else if (hit.collider.tag == "Enemy" || hit.collider.tag == "Civilian")
                    {
                        HitMarker(damage, hit, personType);
                        hit.collider.GetComponent<IDamageable>().TakeDamage(damage, "Body"); 
                    }
                    m_GameManager.IncreaseBulletHit();
                }
            }
            m_BurstCount++;

            if (m_BulletsLeft == 0)
            {
                StopFiring("Effects");
            }
        }
        else
        {
            if (m_Canvas.transform.Find("Out of bullets, reload!") == null)
            {
                m_TextManager.ShowTextCanvas(new Vector2(-450f, -715f), "Out of bullets, reload!", Color.red, new Vector2(600f, 200f));
            }

            if (!m_AudioManager.IsPlayingSound("NoBullets"))
                m_AudioManager.PlaySound("NoBullets");
        }
    }

    private bool CanFire()
    {
        return (m_TimeToFire <= 0f);
    }

    public void StartFiring()
    {
        if (m_BulletsLeft > 0)
        {
            m_Firing = true;
            m_Recoil = 1.0f;
            if (!m_Reloading)
            {
                if (m_GunType == GunType.AK47)
                {
                    string temp = m_GunType + "Shooting";
                    m_AudioManager.PlaySound(temp);
                    m_TempMuzzleFlash = (GameObject)Instantiate(m_MuzzleFlash, m_EndOfBarrel.transform);

                }
                m_Animator.SetBool("Shooting", true);
            }
        }
    }

    public void StopFiring(string type)
    {
        if(type == "All")
        {
            m_Firing = false;
        }

        if (m_GunType == GunType.AK47)
        {
            string temp = m_GunType + "Shooting";
            m_AudioManager.StopSound(temp);
            m_AudioManager.StopSound("BulletHit");
        }
        if (m_TempMuzzleFlash != null && m_GunType == GunType.AK47)
        {
            Destroy(m_TempMuzzleFlash.gameObject);
        }
        m_Animator.SetBool("Shooting", false);
    }

    public void Reload()
    {
        StopFiring("Effects");
        m_Recoil = 0.0f;
        if(m_BulletsLeft != m_MagazineSize)
        {
            string temp = m_GunType + "Reloading";
            m_AudioManager.PlaySound(temp);
            m_Reloading = true;
            m_Animator.SetBool("Reloading", true);
            GetComponent<WeaponSway>().SetActive(false);
        }
    }

    public GunType GetGunType()
    {
        return m_GunType;
    }

    public bool GetNeedToReload()
    {
        if(m_BulletsLeft == 0)
        {
            return true;
        }
        return false;
    }

    public void SetAnimation(string name, bool decision)
    {
        m_Animator.SetBool(name, decision);
    }

    public bool GetAnimation(string name)
    {
        return m_Animator.GetBool(name);
    }

    public void UpdateAmmoCount()
    {
        string temp = m_MagazineSize + " / " + m_MagazineSize;
        m_AmmoCountText.text = temp;
    }

    private void HitMarker(float damage, RaycastHit hit, string personType)
    {
        if (m_HitMarkerGO != null)
        {
            Destroy(m_HitMarkerGO.gameObject);
        }
        m_HitMarkerGO = (GameObject)Instantiate(m_HitMarker);
        m_HitMarkerGO.transform.SetParent(m_Canvas.transform);

        if(personType == "Enemy")
        {
            if (hit.transform.gameObject.GetComponent<Enemy>().GetHealth() - damage <= 0)
            {
                m_HitMarkerGO.GetComponent<Image>().color = Color.red;
            }
            else
            {
                m_HitMarkerGO.GetComponent<Image>().color = Color.white;
            }
        }
        if(personType == "Civilian")
        {
            if (hit.transform.gameObject.GetComponent<Civilian>().GetHealth() - damage <= 0)
            {
                m_HitMarkerGO.GetComponent<Image>().color = Color.red;
            }
            else
            {
                m_HitMarkerGO.GetComponent<Image>().color = Color.yellow;
            }
        }
        Destroy(m_HitMarkerGO.gameObject, 0.167f);
    }

    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

}
