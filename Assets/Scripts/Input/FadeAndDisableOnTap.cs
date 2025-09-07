using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FeatherMoveToFinalPosition : MonoBehaviour
{
    public float slideDuration = 0.5f;
    public float delayBetween = 0.1f;
    public bool AllowReplay = false;

    private List<RectTransform> feathers = new List<RectTransform>();
    private List<Vector2> finalPositions = new List<Vector2>();
    private List<CanvasGroup> featherGroups = new List<CanvasGroup>();
    private bool hasAnimated = false;

    // ✅ This goes here — inside the class, not inside another method
    public IEnumerator PlayFeatherSlide()
    {
        if (hasAnimated && !AllowReplay)
        {
            Debug.Log("FeatherMoveToFinalPosition: Already played once. Skipping.");
            yield break;
        }

        hasAnimated = true;
        feathers.Clear();
        finalPositions.Clear();
        featherGroups.Clear();

        foreach (Transform child in transform)
        {
            RectTransform feather = child.GetComponent<RectTransform>();
            if (feather == null) continue;

            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = child.gameObject.AddComponent<CanvasGroup>();
                Debug.Log("CanvasGroup added to " + child.name);
            }

            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            feathers.Add(feather);
            featherGroups.Add(cg);
        }

        yield return null;

        for (int i = 0; i < feathers.Count; i++)
        {
            finalPositions.Add(feathers[i].anchoredPosition);
            feathers[i].anchoredPosition += new Vector2(0f, 300f);
        }

        for (int i = 0; i < feathers.Count; i++)
        {
            RectTransform feather = feathers[i];
            Vector2 startPos = feather.anchoredPosition;
            Vector2 endPos = finalPositions[i];
            CanvasGroup cg = featherGroups[i];

            float elapsed = 0f;

            while (elapsed < slideDuration)
            {
                float t = elapsed / slideDuration;
                float easedT = Mathf.SmoothStep(0f, 1f, t);
                feather.anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);
                cg.alpha = easedT;

                elapsed += Time.deltaTime;
                yield return null;
            }

            feather.anchoredPosition = endPos;
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            yield return new WaitForSeconds(delayBetween);
        }

        Debug.Log("FeatherMoveToFinalPosition: Float-in animation complete.");
    }
}
