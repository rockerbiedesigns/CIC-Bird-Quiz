using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class AlignTMPTextToParentBottom : MonoBehaviour
{
    public RectTransform targetParent;
    public float bottomPadding = 100f;
    [TextArea]
    public string labelText = "Who Am I?";

    private RectTransform rectTransform;
    private TMP_Text tmp;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tmp = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        if (tmp != null)
        {
            tmp.text = labelText;
        }
    }

    void Update()
    {
        if (rectTransform == null || targetParent == null) return;

        // Anchor to bottom center
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);

        // Position
        rectTransform.anchoredPosition = new Vector2(0f, bottomPadding);
    }
}
