
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideInChildrenSequentially : MonoBehaviour
{
    public float slideDuration = 0.5f;
    public float delayBetweenSlides = 0.3f;
    public float offsetX = 800f;

    public void StartSlideIn()
    {
        StartCoroutine(SlideInCoroutine());
    }

    private IEnumerator SlideInCoroutine()
    {
        List<RectTransform> children = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.gameObject.SetActive(true);
                children.Add(rect);
            }
        }

        foreach (RectTransform rect in children)
        {
            Vector2 endPos = rect.anchoredPosition;
            Vector2 startPos = endPos + new Vector2(offsetX, 0);
            rect.anchoredPosition = startPos;

            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            rect.anchoredPosition = endPos;
            yield return new WaitForSeconds(delayBetweenSlides);
        }
    }
}
