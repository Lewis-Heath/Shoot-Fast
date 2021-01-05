using UnityEngine;

public class WeaponLibary : MonoBehaviour
{
    private static WeaponLibary m_Instance;
    public static WeaponLibary Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("WeaponLibary", typeof(WeaponLibary)).GetComponent<WeaponLibary>();
            }
            return m_Instance;
        }
    }

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            m_Instance = this;
        }
    }

    [SerializeField] private GameObject[] m_GunLibary;

    public GameObject GetWeapon(GunType type)
    {
        foreach(GameObject gun in m_GunLibary)
        {
            if(gun.GetComponent<Gun>().GetGunType() == type)
            {
                return gun;
            }
        }
        Debug.LogWarning("Couldnt find gun " + type.ToString());
        return null;
    }
}