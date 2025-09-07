using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SequentialButtonGameObjectController : MonoBehaviour
{
    [System.Serializable]
    public class TransitionElement
    {
        public Button triggerButton;
        public GameObject fadeOutObject;
        public GameObject fadeInObject;
    }

    public List<TransitionElement> transitions;
    public float fadeDuration = 0.5f;
    public bool dontDeactivate = false;

    private void Start()
    {
        foreach (var t in transitions)
        {
            t.triggerButton.onClick.AddListener(() => StartCoroutine(FadeOut(t)));
        }
    }

    private IEnumerator FadeOut(TransitionElement t)
    {
        CanvasGroup fadeOutGroup = t.fadeOutObject.GetComponent<CanvasGroup>();
        if (fadeOutGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                fadeOutGroup.alpha = 1f - (elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            fadeOutGroup.alpha = 0f;
        }

        // Prevent deactivation if it has QuizDataLoader
        var quizDataLoader = t.fadeOutObject.GetComponent<QuizDataLoader>();
        if (quizDataLoader == null)
        {
            if (!dontDeactivate)
            {
                t.fadeOutObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("[Skip Deactivate] Prevented disabling of GameObject with QuizDataLoader: " + t.fadeOutObject.name);
            if (!quizDataLoader.enabled)
            {
                quizDataLoader.enabled = true;
                Debug.LogWarning("[Recovery] Re-enabled QuizDataLoader script at runtime.");
            }
        }

        if (t.fadeInObject != null)
        {
            t.fadeInObject.SetActive(true);

            CanvasGroup fadeInGroup = t.fadeInObject.GetComponent<CanvasGroup>();
            if (fadeInGroup != null)
            {
                fadeInGroup.alpha = 0f;
                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    fadeInGroup.alpha = elapsed / fadeDuration;
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                fadeInGroup.alpha = 1f;
            }
        }
    }
}
