using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject m_FloatingText;
    [SerializeField] private GameObject m_CanvasText;
    [SerializeField] private GameObject m_Canvas;

    public void ShowTextGame(Vector3 position, Quaternion rotation, string text, Color color)
    {
        position.y += 5;
        GameObject temp = Instantiate(m_FloatingText, position, rotation);
        temp.GetComponent<TextMesh>().text = text;
        temp.GetComponent<TextMesh>().color = color;
    }

    public void ShowTextCanvas(Vector2 screenPosition, string text, Color color, Vector2 size)
    {
        GameObject temp = Instantiate(m_CanvasText);
        temp.transform.SetParent(m_Canvas.transform);
        temp.name = text;
        Text textComp = temp.GetComponent<Text>();
        RectTransform rectTransform = temp.GetComponent<RectTransform>();
        textComp.text = text;
        textComp.color = color;
        rectTransform.localPosition = screenPosition;
        rectTransform.sizeDelta = size;
    }
}
