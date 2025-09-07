using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup quizChoiceCardCanvasGroup;
    [SerializeField] private CanvasGroup questionCanvasGroup;
    [SerializeField] private GameObject quizChoiceCard;
    [SerializeField] private GameObject questionScreen;
    [SerializeField] private QuestionFlowController flowController;
    [SerializeField] private AnimatedButtonSpawner buttonSpawn;

    [Header("UI References for Flow Controller")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private RawImage displayImage;
    [SerializeField] private TextMeshProUGUI commonNameText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform commonNameListContainer;
    [SerializeField] private GameObject commonNameButtonPrefab;
    [SerializeField] private GameObject displayImageParent;

    private void Start()
    {
        // Set flowController references
        flowController.questionPanel = questionPanel;
        flowController.displayImage = displayImage;
        flowController.commonNameText = commonNameText;
        flowController.nextButton = nextButton;
        flowController.commonNameListContainer = commonNameListContainer;
        flowController.commonNameButtonPrefab = commonNameButtonPrefab;
        flowController.displayImage = displayImageParent.GetComponentInChildren<RawImage>();

        // ✅ Subscribe to animation event
        flowController.onQuestionButtonsCreated.AddListener(AnimateAnswerButtons);

    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (flowController != null)
            flowController.onQuestionButtonsCreated.RemoveListener(AnimateAnswerButtons);


    }

    public void StartQuiz(string quizID)
    {
        StartCoroutine(TransitionToQuiz(quizID));
    }

    private IEnumerator TransitionToQuiz(string quizID)
    {
        yield return FadeOut(quizChoiceCard);

        questionCanvasGroup.alpha = 0f;
        questionScreen.SetActive(true);

        var slideAndTilt = displayImage.GetComponentInParent<DisplayImageSlideAndTilt>();
        CanvasGroup transparentImg = displayImage.GetComponentInParent<CanvasGroup>();
        slideAndTilt.ResetPosition();
        slideAndTilt.enabled = true;
        transparentImg.alpha = 0;

        flowController.LoadQuiz(quizID);
        yield return flowController.PreloadFirstQuestionImage();
        yield return flowController.DisplayNextQuestion(); // Animation will be triggered via event

        if (slideAndTilt != null)
        {
            slideAndTilt.StartSlide();
        }

        yield return FadeIn(questionCanvasGroup);
    }

    private IEnumerator FadeOut(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0;
        go.SetActive(false);
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;
    }

    // ✅ Public animation method triggered via event
    public void AnimateAnswerButtons()
    {
        buttonSpawn.AnimateExistingButtons();
    }
}
