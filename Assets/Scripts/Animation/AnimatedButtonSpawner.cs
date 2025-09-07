using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedButtonSpawner : MonoBehaviour
{
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float animationDelay = 0.05f;
    [SerializeField] private float slideOffset = 500f;

    private List<RectTransform> rectsToAnimate = new List<RectTransform>();

    public void AnimateExistingButtons()
    {
        StopAllCoroutines();
        rectsToAnimate.Clear();
        StartCoroutine(AnimateAfterLayout());
    }

    private IEnumerator AnimateAfterLayout()
    {
        // Wait for layout to complete
        yield return new WaitForEndOfFrame();

        int index = 0;
        foreach (Transform child in transform)
        {
            RectTransform rect = child as RectTransform;
            if (rect == null) continue;

            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = child.gameObject.AddComponent<CanvasGroup>();

            rectsToAnimate.Add(rect);

            Vector2 targetPos = rect.anchoredPosition;
            Vector2 startPos = targetPos + new Vector2(slideOffset, 0);
            rect.anchoredPosition = startPos;
            cg.alpha = 0;

            StartCoroutine(AnimateSingleButton(rect, targetPos, cg, index * animationDelay));
            index++;
        }
    }


    private IEnumerator AnimateSingleButton(RectTransform rect, Vector2 targetPos, CanvasGroup cg, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        Vector2 startPos = rect.anchoredPosition;

        while (elapsed < slideDuration)
        {
            if (rect == null || cg == null) yield break;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (rect != null) rect.anchoredPosition = targetPos;

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (cg == null) yield break;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (cg != null) cg.alpha = 1f;
    }
}
