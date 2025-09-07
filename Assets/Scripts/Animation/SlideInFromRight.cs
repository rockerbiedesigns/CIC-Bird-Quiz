using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlideInFromRight : MonoBehaviour
{
    public float duration = 0.5f;
    public float offsetX = 800f;
    public float delayBetweenItems = 0.3f;

    private List<RectTransform> childRects = new List<RectTransform>();

    void Start()
    {
        // Collect and deactivate all children
        foreach (Transform child in transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                childRects.Add(rect);
                child.gameObject.SetActive(false);
            }
        }

        StartCoroutine(SlideInChildrenOneAtATime());
    }

    IEnumerator SlideInChildrenOneAtATime()
    {
        foreach (RectTransform rect in childRects)
        {
            rect.gameObject.SetActive(true);
            Vector2 startPos = rect.anchoredPosition + new Vector2(offsetX, 0);
            Vector2 endPos = rect.anchoredPosition;

            rect.anchoredPosition = startPos;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            rect.anchoredPosition = endPos;

            yield return new WaitForSeconds(delayBetweenItems);
        }
    }
}
