using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using System.Linq;

public class QuestionFlowController : MonoBehaviour
{
    [HideInInspector] public GameObject quizSelectionPanel;
    [HideInInspector] public GameObject questionPanel;
    [HideInInspector] public RawImage displayImage;
    [HideInInspector] public TextMeshProUGUI commonNameText;
    [SerializeField] public TextMeshProUGUI finalScoreText;
    [HideInInspector] public Button nextButton;
    [SerializeField] public Transform commonNameListContainer;
    [SerializeField] public GameObject commonNameButtonPrefab;
    [Header("Quiz Settings")]
    public int maxQuestions = 2;

    [Header("Fade Targets")]
    [SerializeField] private GameObject questionScreenObject;
    [SerializeField] private GameObject scorePageObject;
    public int Score => score;
    public int TotalQuestions => currentThingIDs?.Count ?? 0;


    private List<string> currentThingIDs;
    private int currentQuestionIndex = 0;
    private string currentQuizID;

    private int score = 0;
    private string currentThingID; // Shared ThingID for current question


    private AnswerDisplayManager answerDisplayManager;

    private bool nextButtonShown = false;
    public UnityEvent onQuestionButtonsCreated;


    void Awake()
    {
        Debug.Log($"[Awake] maxQuestions = {maxQuestions}");
    }

    void Start()
    {
        Debug.Log($"[Start] maxQuestions = {maxQuestions}");
    }


    public IEnumerator PreloadFirstQuestionImage()
    {
        if (QuizDataCache.Instance.quizIDToThingIDs.TryGetValue(currentQuizID, out List<string> ids) && ids.Count > 0)
        {
            string thingID = ids[0];

            if (QuizDataCache.Instance.thingIDToMediaID.TryGetValue(thingID, out int mediaID))
            {
                string imagePath = QuizDataCache.Instance.GetMediaPath(mediaID.ToString());

                if (!string.IsNullOrEmpty(imagePath))
                {
                    var slideAndTilt = displayImage.GetComponent<DisplayImageSlideAndTilt>();

                    if (slideAndTilt != null)
                    {
                        slideAndTilt.ResetPosition(); // reset position before loading new image
                        slideAndTilt.enabled = false;
                    }

                    displayImage.gameObject.SetActive(false); // deactivate to refresh
                    yield return StartCoroutine(QuizImageLoader.LoadImage(displayImage, imagePath));
                    displayImage.gameObject.SetActive(true);  // force re-render
                    Canvas.ForceUpdateCanvases();
                    yield return null;

                    if (slideAndTilt != null)
                    {
                        slideAndTilt.enabled = true;
                        slideAndTilt.StartSlide(); // re-trigger animation on new image
                    }
                }

            }
        }

        Debug.LogWarning("[QuestionFlowController] Failed to preload first image.");
        yield break;
    }


    public void LoadQuiz(string quizID)
    {
        currentQuizID = quizID;
        Debug.Log($"[QuestionFlowController] maxQuestions = {maxQuestions}");

        if (!QuizDataCache.Instance.quizIDToThingIDs.ContainsKey(quizID))
        {
            Debug.LogError($"[QuestionFlowController] QuizID '{quizID}' not found in cache.");
            return;
        }

        List<string> allIDs = new List<string>(QuizDataCache.Instance.quizIDToThingIDs[quizID]);
        Shuffle(allIDs);

        HashSet<string> selected = new HashSet<string>();
        currentThingIDs = new List<string>();
        foreach (string id in allIDs)
        {
            if (!selected.Contains(id))
            {
                selected.Add(id);
                currentThingIDs.Add(id);
            }
            if (currentThingIDs.Count >= maxQuestions)
                break;
        }

        currentQuestionIndex = 0;
        score = 0;

        answerDisplayManager = FindFirstObjectByType<AnswerDisplayManager>();
        if (answerDisplayManager == null)
        {
            Debug.LogError("[QuestionFlowController] AnswerDisplayManager not found.");
        }
    }

    public IEnumerator DisplayNextQuestion()
    {
        nextButtonShown = false;

        if (currentThingIDs == null || currentThingIDs.Count == 0)
        {
            Debug.LogWarning("[QuestionFlowController] currentThingIDs is null or empty at DisplayNextQuestion.");
            yield break;
        }

        if (currentQuestionIndex >= currentThingIDs.Count)
        {
            Debug.Log("[QuestionFlowController] Quiz complete.");
            yield return StartCoroutine(FadeToScorePage());
            yield break;
        }

        if (nextButton != null)
            nextButton.interactable = false;

        string thingID = currentThingIDs[currentQuestionIndex];

        string mediaID = QuizDataCache.Instance.thingIDToMediaID.ContainsKey(thingID)
            ? QuizDataCache.Instance.thingIDToMediaID[thingID].ToString()
            : null;

        if (!string.IsNullOrEmpty(mediaID))
        {
            string imagePath = QuizDataCache.Instance.GetMediaPath(mediaID);
            if (!string.IsNullOrEmpty(imagePath))
            {
                // Disable animation during loading
                var slideAndTilt = displayImage.GetComponentInParent<DisplayImageSlideAndTilt>();
                if (slideAndTilt != null) slideAndTilt.enabled = false;

                yield return StartCoroutine(QuizImageLoader.LoadImage(displayImage, imagePath));
                yield return null;
                Canvas.ForceUpdateCanvases();

                if (slideAndTilt != null) slideAndTilt.enabled = true;
            }
            else
            {
                Debug.LogWarning($"[QuestionFlowController] No image path found for MediaID '{mediaID}'");
            }
        }
        else
        {
            Debug.LogWarning($"[QuestionFlowController] No MediaID found for ThingID '{thingID}'");
        }

        string correctName = QuizDataCache.Instance.GetCommonName(thingID) ?? "Unknown";

        commonNameText.text = "";

        foreach (Transform child in commonNameListContainer)
        {
            Destroy(child.gameObject);
        }

        List<string> incorrectNames = GetIncorrectNames(thingID);
        List<string> allNames = new List<string>(incorrectNames);
        allNames.Add(correctName);
        Shuffle(allNames);

        Debug.Log("[QuestionFlowController] Correct Answer: " + correctName);
        foreach (var name in incorrectNames)
        {
            Debug.Log("[QuestionFlowController] Incorrect Option: " + name);
        }

        answerDisplayManager.SetCorrectAnswer(correctName);
        answerDisplayManager.SetFlowController(this);

        // Prepare Next Button for fade-in
        if (nextButton != null)
        {
            CanvasGroup cg = nextButton.GetComponent<CanvasGroup>();
            if (cg == null) cg = nextButton.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = true;
        }

foreach (string name in allNames)
{
    string localizedName = name;

    if (LanguageController.Instance.IsSpanish())
    {
        // Debug the lookup process
        Debug.Log($"[Localization] Looking for Spanish version of: '{name}'");

        if (QuizDataCache.Instance.commonNameToCommonName_es.TryGetValue(name.Trim(), out string name_es))
        {
            localizedName = name_es;
            Debug.Log($"[Localization] Found Spanish: '{name_es}' for '{name}'");
        }
        else
        {
            Debug.LogWarning($"[Localization] Missing Spanish for: '{name}'");
        }
    }

    GameObject btnObj = Instantiate(commonNameButtonPrefab, commonNameListContainer);
    TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
    if (txt != null)
    {
        txt.text = localizedName;
    }

Button btn = btnObj.GetComponent<Button>();
if (btn != null)
{
    Button capturedBtn = btn;
    string internalEnglishName = name; // non-localized
    capturedBtn.onClick.AddListener(() =>
    {
        answerDisplayManager.RegisterSelectedButton(capturedBtn, internalEnglishName);
    });
}
}

        StartCoroutine(FadeInNextButton());

        // Only register once
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            answerDisplayManager.OnNextButtonPressed();
        });

        // ✅ Animate image parent AFTER image is ready
        Transform imageParent = displayImage.transform.parent;
        if (imageParent != null)
        {
            var slideAndTilt = imageParent.GetComponent<DisplayImageSlideAndTilt>();
            if (slideAndTilt != null)
            {
                slideAndTilt.PlaySlideIn();
            }
        }

        Debug.Log($"[QuestionFlowController] Instantiated {allNames.Count} answer buttons.");
        onQuestionButtonsCreated?.Invoke();

    }
    public bool IsLastQuestion()
    {
        return currentQuestionIndex >= currentThingIDs.Count - 1;
    }


    public void IncrementScore()
    {
        score++;
        Debug.Log("[QuestionFlowController] Score incremented to: " + score);
    }
    private IEnumerator HandleNextQuestionWithDelay()
    {
        yield return new WaitForSeconds(1f); // allow user to see result
        currentQuestionIndex++;
        if (currentQuestionIndex < currentThingIDs.Count)
        {
            currentThingID = currentThingIDs[currentQuestionIndex];
            yield return StartCoroutine(DisplayNextQuestion());
        }
        else
        {
            Debug.Log("[HandleNextQuestionWithDelay] Reached end of questions.");
            yield return StartCoroutine(FadeToScorePage());
        }
    }

    private IEnumerator FadeToScorePage()
{
    if (questionScreenObject != null)
        questionScreenObject.SetActive(false);
    else
        Debug.LogError("[QuestionFlowController] questionScreenObject is not assigned!");

    if (scorePageObject != null)
    {
        scorePageObject.SetActive(true);
        var cg = scorePageObject.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }
    else
        Debug.LogError("[QuestionFlowController] scorePageObject is not assigned!");

    // ✅ Let DisplayFinalScoreUI handle this instead
    /*
    if (finalScoreText != null && currentThingIDs != null)
    {
        finalScoreText.text = $"You got {score} out of {currentThingIDs.Count} correct!";
        finalScoreText.gameObject.SetActive(true);
    }
    else
    {
        if (finalScoreText == null) Debug.LogError("[QuestionFlowController] finalScoreText is null!");
        if (currentThingIDs == null) Debug.LogError("[QuestionFlowController] currentThingIDs is null!");
    }
    */

    if (nextButton != null)
        nextButton.gameObject.SetActive(false);

    yield return null;
}



    private List<string> GetIncorrectNames(string thingID)
    {
        HashSet<string> usedNames = new HashSet<string>();
        List<string> wrongNames = new List<string>();

        if (QuizDataCache.Instance.thingIDToSimilarThingIDs.TryGetValue(thingID, out List<string> similarThingIDs))
        {
            Shuffle(similarThingIDs);
            foreach (string altThingID in similarThingIDs)
            {
                string name = QuizDataCache.Instance.GetCommonName(altThingID);
                if (!string.IsNullOrEmpty(name) && !usedNames.Contains(name))
                {
                    wrongNames.Add(name);
                    usedNames.Add(name);
                    if (wrongNames.Count == 3)
                        return wrongNames;
                }
            }
        }

        // Fallback if fewer than 3
        List<string> fallbackPool = new List<string>();
        foreach (var kvp in QuizDataCache.Instance.thingIDToCommonNameID.Keys)
        {
            string name = QuizDataCache.Instance.GetCommonName(kvp);
            if (!string.IsNullOrEmpty(name) && !usedNames.Contains(name) && kvp != thingID)
            {
                fallbackPool.Add(name);
            }
        }

        Shuffle(fallbackPool);

        foreach (string name in fallbackPool)
        {
            wrongNames.Add(name);
            usedNames.Add(name);
            if (wrongNames.Count == 3)
                break;
        }

        return wrongNames;
    }

    private void DisplayAnswerResult(bool correct, string correctName)
    {
        commonNameText.text = correct ? $"Correct! It's {correctName}." : $"Oops! It was {correctName}.";
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randIndex = Random.Range(i, list.Count);
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
    public void ResetQuizState()
    {
        currentThingIDs = null;
        currentQuestionIndex = 0;
        currentQuizID = null;
        score = 0;
    }
    private IEnumerator FadeInNextButton()
    {
        if (nextButtonShown) yield break;
        nextButtonShown = true;

        CanvasGroup cg = nextButton.GetComponent<CanvasGroup>();
        if (cg == null) cg = nextButton.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float t = 0f;
        while (t < 1f)
        {
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            t += Time.deltaTime / 0.4f;
            yield return null;
        }

        cg.alpha = 0.33f;
        cg.interactable = true;     // ✅ Critical
        cg.blocksRaycasts = true;   // ✅ Needed for clicks to register
        nextButton.interactable = true;
    }
    public void AdvanceQuestionIndex()
    {
        currentQuestionIndex++;
    }

    private string GetLocalizedCommonName(string name)
    {
        if (!LanguageController.Instance.IsSpanish())
            return name;

        foreach (var kvp in QuizDataCache.Instance.commonNameIDToCommonName)
        {
            if (kvp.Value == name)
            {
                string commonNameID = kvp.Key;
                if (QuizDataCache.Instance.commonNameIDToCommonName_es.TryGetValue(commonNameID, out string name_es))
                {
                    return name_es;
                }
            }
        }
        return name;
    }

}