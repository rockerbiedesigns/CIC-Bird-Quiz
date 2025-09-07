using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestartQuizButtonHandler : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject scorePagePanel;
    [SerializeField] private GameObject titleCardPanel;
    [SerializeField] private GameObject quizChoiceCardPanel;

    [Header("Quiz Components")]
    [SerializeField] private QuestionFlowController flowController;
    [SerializeField] private Transform categoryButtonParent;
    [SerializeField] private Transform quizListContainer;

    [Header("Fade Timing")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Buttons to Reset (Interactable Only)")]
    [SerializeField] private List<Button> buttonsToReset;

    [Header("Panels to Reset")]
    public GameObject scorePage;
    public GameObject titleCard;
    public GameObject quizChoiceCard;
    public GameObject questionScreen;

    [Header("Containers with Instantiated Buttons")]
    public Transform categoryButtonContainer;
    public Transform answerButtonContainer;

    [Header("Resettable UI Components")]
    public TextMeshProUGUI finalScoreText;

    [Header("Flow Controller")]
    public QuestionFlowController questionFlowController;

    private CanvasGroup scoreCanvasGroup;
    private CanvasGroup titleCanvasGroup;
    private CanvasGroup quizChoiceCanvasGroup;

    private void Start()
    {
        scoreCanvasGroup = GetOrAddCanvasGroup(scorePagePanel);
        titleCanvasGroup = GetOrAddCanvasGroup(titleCardPanel);
        quizChoiceCanvasGroup = GetOrAddCanvasGroup(quizChoiceCardPanel);
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        if (obj == null) return null;
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }

    public void RestartQuiz()
    {
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        if (scoreCanvasGroup != null)
            yield return StartCoroutine(FadeOut(scoreCanvasGroup));
        scorePagePanel.SetActive(false);

        if (flowController != null)
        {
            flowController.ResetQuizState();
// Reset the image slide position to avoid ghost image before animation
if (flowController.displayImage != null)
{
    // Step 1: Clear image
    flowController.displayImage.texture = null;

    // Step 2: Disable the RawImage GameObject to avoid rendering it
    flowController.displayImage.gameObject.SetActive(false);

    // ✅ Step 3: Reset the parent object's animation
    Transform imageParent = flowController.displayImage.transform.parent;
    if (imageParent != null)
    {
        var slideAndTilt = imageParent.GetComponent<DisplayImageSlideAndTilt>();
        if (slideAndTilt != null)
        {
            slideAndTilt.ResetPosition();
            slideAndTilt.enabled = false;
        }
        else
        {
            Debug.LogWarning("[Restart] SlideAndTilt component not found on display image parent.");
        }
    }
    else
    {
        Debug.LogWarning("[Restart] DisplayImage parent not found.");
    }
}



            ClearChildren(flowController.commonNameListContainer);
            ClearChildren(categoryButtonParent);
            ClearChildren(quizListContainer);

            if (flowController.nextButton != null)
            {
                flowController.nextButton.gameObject.SetActive(true);

                CanvasGroup cg = flowController.nextButton.GetComponent<CanvasGroup>();
                if (cg == null) cg = flowController.nextButton.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;

                flowController.nextButton.interactable = false;
            }

        }

        foreach (Button btn in buttonsToReset)
        {
            if (btn != null)
            {
                btn.interactable = true;
            }
        }

        titleCardPanel.SetActive(true);
        if (titleCanvasGroup != null)
            yield return StartCoroutine(FadeIn(titleCanvasGroup));
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnRestartQuizPressed()
    {
        scorePage.SetActive(false);
        questionScreen.SetActive(false);
        titleCard.SetActive(true);
        quizChoiceCard.SetActive(false);

        ClearChildren(categoryButtonContainer);
        ClearChildren(quizListContainer);
        ClearChildren(answerButtonContainer);

        foreach (var button in buttonsToReset)
        {
            if (button != null)
            {
                button.interactable = true;
            }
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "";
            finalScoreText.gameObject.SetActive(false);
        }

        if (questionFlowController != null)
        {
            questionFlowController.ResetQuizState();
            QuizDataCache.Instance.ClearCache();
        }

        PlayerPrefs.DeleteKey("SelectedQuizID");
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
