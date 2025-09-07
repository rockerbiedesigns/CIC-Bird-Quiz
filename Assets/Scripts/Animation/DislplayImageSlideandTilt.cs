using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayImageSlideAndTilt : MonoBehaviour
{
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideDistance = 300f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;
    }

public void StartSlide()
{
    if (rectTransform == null) return;

    StopAllCoroutines();
    StartCoroutine(SlideAndRotateCoroutine());
}


    private IEnumerator SlideIn()
    {
        float elapsed = 0f;
        Vector2 offscreenPosition = originalPosition + new Vector2(slideDistance, 0); // ✅ define offscreen
        Vector2 endPosition = originalPosition;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(offscreenPosition, endPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPosition;

        // ✅ Apply random Z-rotation between -7 and 7 degrees
        float randomZ = Random.Range(-7f, 7f);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, randomZ);
    }

    private IEnumerator SlideAndRotateCoroutine()
{
    float elapsed = 0f;
    Vector2 startPos = rectTransform.anchoredPosition;
    Vector2 endPos = originalPosition;

    float startAngle = 0f;
    float targetAngle = Random.Range(-7f, 7f);

    while (elapsed < slideDuration)
    {
        float t = elapsed / slideDuration;

        // Slide position
        rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

        // Smooth angle rotation
        float angle = Mathf.Lerp(startAngle, targetAngle, t);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);

        elapsed += Time.deltaTime;
        yield return null;
    }

    // Ensure final position and rotation are exact
    rectTransform.anchoredPosition = endPos;
    rectTransform.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
}


    public void PlaySlideIn()
    {
        ResetPosition();
        enabled = true;
        StartSlide();
    }

public void ResetPosition()
{
    if (rectTransform != null)
    {
        rectTransform.anchoredPosition = originalPosition + new Vector2(slideDistance, 0);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // ✅ Reset alpha if there's a CanvasGroup on this GameObject
    CanvasGroup cg = GetComponent<CanvasGroup>();
    if (cg != null)
    {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}

}
