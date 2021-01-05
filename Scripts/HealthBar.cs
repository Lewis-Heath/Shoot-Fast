using UnityEngine;

public class HealthBar : MonoBehaviour
{
    void Update()
    {
        Transform cameraRot = Camera.main.transform;
        transform.rotation = Quaternion.LookRotation(cameraRot.forward);
    }
}
