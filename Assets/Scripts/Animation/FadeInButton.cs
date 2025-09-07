using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInButton : MonoBehaviour
{
    public float delay = 0f;
    public float duration = 0.3f;

    private CanvasGroup cg;

    void Awake()
    {
        cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
    }

    void OnEnable()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }
}
