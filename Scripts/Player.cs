using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform m_Gun;
    [SerializeField] private Transform m_AttachPoint;
    [SerializeField] private Transform m_AttachPoint2;
    [SerializeField] private CharacterController m_Controller;
    [SerializeField] private float m_WalkingSpeed;
    [SerializeField] private float m_SprintSpeed;
    [SerializeField] private float m_CrouchSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    [SerializeField] private GameObject m_Grenade;
    [SerializeField] private TextManager m_TextManager;

    [SerializeField] private GameObject m_Canvas;
    
    [SerializeField] private Transform groundCheck;
    private float groundDistance = 0.4f;
    [SerializeField] private Transform roofCheck;
    private float roofDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;
    private float m_OriginalSlopeLimit;

    private GameObject m_GunEquippedImage;
    private GameObject m_GrenadesImage;

    private int m_GrenadeCount = 3;

    private float m_Speed;
    private Vector3 velocity;

    private float m_OriginalHeight;
    [SerializeField] private float m_CrouchHeight = 1.0f;
    private bool m_Crouching = false;

    private AudioManager m_AudioManager;
    private GameManager m_GameManager;

    private GameObject m_Camera;

    private bool m_Walking = false;
    private bool m_Running = false;

    private float m_SwapTime = 0.0f;
    private bool m_SwappingWeaponAK47 = false;
    private bool m_SwappingWeaponM107 = false;

    private GunType m_GunEquipped;


    void Start()
    {
        m_Speed = m_WalkingSpeed;
        m_OriginalHeight = m_Controller.height;
        m_GunEquippedImage = GameObject.FindGameObjectWithTag("GunEquipped");
        m_GrenadesImage = GameObject.FindGameObjectWithTag("GrenadesCounter");
        m_AudioManager = FindObjectOfType<AudioManager>();
        m_GameManager = FindObjectOfType<GameManager>();
        m_GunEquipped = GunType.M107;
        EquipWeapon(WeaponLibary.Instance.GetWeapon(GunType.M107));
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera");
        m_OriginalSlopeLimit = m_Controller.slopeLimit;
    }

    void Update()
    {
        if(m_GameManager.m_CurrentScreen == ScreenType.Playing)
        {
            if (m_SwappingWeaponAK47)
            {
                if (m_SwapTime > 2.75f)
                {
                    EquipWeapon(WeaponLibary.Instance.GetWeapon(GunType.AK47));
                    m_SwapTime = 0.0f;
                    m_SwappingWeaponAK47 = false;
                    m_Canvas.transform.Find("SwappingWeapons").gameObject.SetActive(false);
                }
                else
                {
                    m_SwapTime += Time.deltaTime;
                }
            }

            if (m_SwappingWeaponM107)
            {
                if (m_SwapTime > 2.75f)
                {
                    EquipWeapon(WeaponLibary.Instance.GetWeapon(GunType.M107));
                    m_SwapTime = 0.0f;
                    m_SwappingWeaponM107 = false;
                    m_Canvas.transform.Find("SwappingWeapons").gameObject.SetActive(false);
                }
                else
                {
                    m_SwapTime += Time.deltaTime;
                }
            }


            if (m_Gun != null)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    m_Gun.GetComponent<IFireable>().StartFiring();
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    m_Gun.GetComponent<IFireable>().StopFiring("All");
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    m_Gun.GetComponent<IFireable>().Reload();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (m_GunEquipped != GunType.AK47 && !m_SwappingWeaponAK47)
                {
                    StartWeaponSwap(GunType.AK47);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (m_GunEquipped != GunType.M107 && !m_SwappingWeaponM107)
                {
                    StartWeaponSwap(GunType.M107);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                UnEquipWeapon();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (m_GrenadeCount > 0)
                {
                    string currentGrenade = "Grenade" + m_GrenadeCount;
                    GameObject temp = m_GrenadesImage.transform.Find(currentGrenade).gameObject;
                    temp.SetActive(false);
                    m_GrenadeCount--;
                    Instantiate(m_Grenade, m_Camera.transform.position, m_Camera.transform.rotation);
                }
                else
                {
                    if (m_Canvas.transform.Find("Out of Grenades!") == null)
                    {
                        m_TextManager.ShowTextCanvas(new Vector2(1075f, -715f), "Out of Grenades!", Color.red, new Vector2(600f, 200f));
                    }
                    if (!m_AudioManager.IsPlayingSound("NoBullets"))
                        m_AudioManager.PlaySound("NoBullets");
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                m_Crouching = !m_Crouching;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                m_Running = true;
                m_Speed = m_SprintSpeed;
                if (m_Gun != null)
                {
                    m_Gun.GetComponent<Gun>().SetAnimation("Running", true);
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                m_Running = false;
                m_Speed = m_WalkingSpeed;
                if (m_Gun != null)
                {
                    m_Gun.GetComponent<Gun>().SetAnimation("Running", false);
                }
            }

            CheckIfWalking();

            if (m_Walking)
            {
                if (m_Running)
                {
                    if (!m_AudioManager.IsPlayingSound("Running") && !m_AudioManager.IsPlayingSound("Jump") && isGrounded)
                    {
                        m_AudioManager.RandomizePitchSound("Running");
                        m_AudioManager.PlaySound("Running");
                    }
                }
                else
                {
                    if (!m_AudioManager.IsPlayingSound("Walking") && !m_AudioManager.IsPlayingSound("Jump") && isGrounded)
                    {
                        m_AudioManager.RandomizePitchSound("Walking");
                        m_AudioManager.PlaySound("Walking");
                    }
                }
            }
            else
            {
                m_AudioManager.StopSound("Walking");
                m_AudioManager.StopSound("Running");
            }

            CheckCrouch();

            if (!m_AudioManager.IsPlayingSound("Breathing") && isGrounded)
                m_AudioManager.PlaySound("Breathing");

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                m_Controller.slopeLimit = m_OriginalSlopeLimit;
            }

            if (Physics.CheckSphere(roofCheck.position, roofDistance, groundMask))
            {
                velocity.y = -9.81f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            m_Controller.Move(move * m_Speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                m_AudioManager.StopSound("Breathing");
                m_AudioManager.StopSound("Walking");
                m_AudioManager.RandomizePitchSound("Jump");
                m_AudioManager.PlaySound("Jump");
                m_AudioManager.RandomizePitchSound("JumpGrunt");
                m_AudioManager.PlaySound("JumpGrunt");
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //m_Controller.slopeLimit = 90.0f;
            }

            velocity.y += gravity * Time.deltaTime;
            m_Controller.Move(velocity * Time.deltaTime);
        }
    }

    public void EquipWeapon(GameObject weapon)
    {
        IFireable temp = weapon.GetComponent<IFireable>();
        if (temp != null)
        {
            if(weapon.name == "AK47")
            {
                GameObject tempGO = Instantiate<GameObject>(weapon, m_AttachPoint);
                m_Gun = tempGO.transform;
            }
            if (weapon.name == "M107")
            {
                GameObject tempGO = Instantiate<GameObject>(weapon, m_AttachPoint2);
                m_Gun = tempGO.transform;
            }
        }

        if(temp.GetGunType() == GunType.AK47)
        {
            m_GunEquippedImage.transform.Find("AK47").gameObject.SetActive(true);
        }
        if (temp.GetGunType() == GunType.M107)
        {
            m_GunEquippedImage.transform.Find("M107").gameObject.SetActive(true);
        }
    }

    public void UnEquipWeapon()
    {
        if (m_Gun != null)
        {
            Destroy(m_Gun.gameObject);
        }
    }

    private void CheckIfWalking()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            m_Walking = true;
            if (m_Gun != null)
            {
                m_Gun.GetComponent<Gun>().SetAnimation("Walking", true);
            }

        }
        if (Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.S) == false)
        {
            m_Walking = false;
            if(m_Gun != null)
            {
                m_Gun.GetComponent<Gun>().SetAnimation("Walking", false);
                if (m_Gun.GetComponent<Gun>().GetAnimation("Running") == true)
                    m_Gun.GetComponent<Gun>().SetAnimation("Running", false);
            }
        }
    }

    private void CheckCrouch()
    {
        if(m_Crouching == true)
        {
            if(m_Speed == m_WalkingSpeed || m_Speed == m_SprintSpeed)
            {
                m_Speed = m_CrouchSpeed;
            }
            if(m_Controller.height != m_CrouchHeight)
            {
                if (m_Controller.height < m_CrouchHeight)
                {
                    m_Controller.height = m_CrouchHeight;
                }
                else
                {
                    m_Controller.height -= 8f * Time.deltaTime;
                }
            }
        }
        else
        {
            if (m_Speed == m_CrouchSpeed)
            {
                m_Speed = m_WalkingSpeed;
            }
            if (m_Controller.height != m_OriginalHeight)
            {
                if (m_Controller.height > m_OriginalHeight)
                {
                    m_Controller.height = m_OriginalHeight;
                }
                else
                {
                    m_Controller.height += 8f * Time.deltaTime;
                }
            } 
        }
    }

    private void StartWeaponSwap(GunType gun)
    {
        StopShootingSounds();
        UnEquipWeapon();
        m_GunEquipped = gun;
        transform.Find("Main Camera").GetComponent<MouseMovement>().SetRecoil(0.0f);

        if(gun == GunType.AK47)
        {
            m_SwappingWeaponAK47 = true;
            m_SwappingWeaponM107 = false;
        }
        if (gun == GunType.M107)
        {
            m_SwappingWeaponAK47 = false;
            m_SwappingWeaponM107 = true;
        }

        m_AudioManager.PlaySound("WeaponSwap");
        m_Canvas.transform.Find("SwappingWeapons").gameObject.SetActive(true);
        m_GunEquippedImage.transform.Find("M107").gameObject.SetActive(false);
        m_GunEquippedImage.transform.Find("AK47").gameObject.SetActive(false);
    }

    private void StopShootingSounds()
    {
        if(m_GunEquipped == GunType.AK47)
        {
            if(m_AudioManager.IsPlayingSound("AK47Shooting"))
                m_AudioManager.StopSound("AK47Shooting");
        }
    }
}
