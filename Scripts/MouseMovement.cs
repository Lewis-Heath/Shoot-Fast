﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float m_Sensitivity;
    public Transform m_PlayerBody;
    float xRotation = 0.0f;
    float xRecoil = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * m_Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_Sensitivity * Time.deltaTime;

        xRotation += -mouseY + xRecoil;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        m_PlayerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetRecoil(float recoilSeverity)
    {
        //xRecoil = 0;
        xRecoil = -recoilSeverity * Time.deltaTime;
    }
}
