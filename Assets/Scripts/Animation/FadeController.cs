using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public static IEnumerator FadeOut(CanvasGroup canvasGroup, float duration = 0.3f)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public static void DisableObject(GameObject go)
    {
        if (go != null)
            go.SetActive(false);
    }
}
