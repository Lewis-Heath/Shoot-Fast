using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    [SerializeField] private float m_Severity;
    [SerializeField] private float m_Smoothness;
    [SerializeField] private float m_MaxSway;
    [SerializeField] private GunType m_GunType;
    private bool m_Active = true;
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;

    void Start()
    {
        m_InitialPosition = transform.localPosition;
        m_InitialRotation = transform.localRotation;
    }

    void Update()
    {
        if(m_Active)
        {
            float mouseX = Input.GetAxis("Mouse X") * m_Severity;
            float mouseY = Input.GetAxis("Mouse Y") * m_Severity;
            mouseX = Mathf.Clamp(mouseX, -m_MaxSway, m_MaxSway);
            mouseY = Mathf.Clamp(mouseY, -m_MaxSway, m_MaxSway);

            Vector3 finalPosition = new Vector3(mouseX, mouseY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + m_InitialPosition, Time.deltaTime * m_Smoothness);


            float mouseRotX = 0.0f;
            float mouseRotY = 0.0f;

            if (m_GunType == GunType.AK47)
            {
                mouseRotX = Input.GetAxis("Mouse X") * -m_Severity;
                mouseRotY = Input.GetAxis("Mouse Y") * -m_Severity;
            }
            else if (m_GunType == GunType.M107)
            {
                mouseRotX = Input.GetAxis("Mouse X") * m_Severity;
                mouseRotY = Input.GetAxis("Mouse Y") * m_Severity;
            }
            
            mouseRotX = Mathf.Clamp(mouseRotX, -m_MaxSway, m_MaxSway);
            mouseRotY = Mathf.Clamp(mouseRotY, -m_MaxSway, m_MaxSway);

            Quaternion finalRotation = new Quaternion(0f, 0f, mouseRotX, 1f);
            finalRotation.z += m_InitialRotation.z;
            finalRotation.y += m_InitialRotation.y;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * m_Smoothness);
        }
    }

    public void SetActive(bool decision)
    {
        m_Active = decision;
    }
}
