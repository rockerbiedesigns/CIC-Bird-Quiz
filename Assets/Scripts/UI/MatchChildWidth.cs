using UnityEngine;

[ExecuteAlways]
public class MatchChildWithBottomPadding : MonoBehaviour
{
    public RectTransform child;

    [Header("Padding")]
    public float horizontalPadding = 10f;
    public float topPadding = 10f;
    public float bottomPadding = 100f;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (child == null || rectTransform == null) return;

        float paddedWidth = child.rect.width + horizontalPadding * 2f;
        float paddedHeight = child.rect.height + topPadding + bottomPadding;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, paddedWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, paddedHeight);

        // ⚠️ Anchor the child to the top-middle
        child.anchorMin = new Vector2(0.5f, 1f);
        child.anchorMax = new Vector2(0.5f, 1f);
        child.pivot = new Vector2(0.5f, 1f);

        // 🧷 Offset child down from top by topPadding
        child.anchoredPosition = new Vector2(0f, -topPadding);
    }
}
