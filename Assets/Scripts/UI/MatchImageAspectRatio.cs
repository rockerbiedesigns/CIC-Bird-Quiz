using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RawImage))]
public class MatchImageAspectRatio : MonoBehaviour
{
    [Tooltip("Maximum height for the image (in pixels)")]
    public float maxHeight = 500f;

    private RawImage rawImage;
    private RectTransform rectTransform;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rawImage.texture == null || rectTransform == null)
            return;

        float imageWidth = rawImage.texture.width;
        float imageHeight = rawImage.texture.height;

        if (imageHeight == 0) return; // Prevent divide-by-zero

        float aspectRatio = imageWidth / imageHeight;

        // Use max height, or the parent's height if smaller
        float targetHeight = Mathf.Min(maxHeight, rectTransform.rect.height);
        float newWidth = targetHeight * aspectRatio;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }
}
