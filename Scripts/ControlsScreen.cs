using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScreen : MonoBehaviour
{
    public void MenuSelected()
    {
        Debug.Log("Menu");
        gameObject.SetActive(false);
    }
}
