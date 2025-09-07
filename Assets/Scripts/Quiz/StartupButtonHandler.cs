using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class StartupButtonHandler : MonoBehaviour
{
    [Header("References")]
    public Button startButton;
    public GameObject titleCard;
    public GameObject quizChoiceCard;
    public Transform categoryButtonParent;
    public GameObject categoryButtonPrefab;
    [SerializeField]
    private CategoryButtonSelector categorySelector;

    [Header("Quiz List UI")]
    public Transform quizListContainer;
    public GameObject quizItemPrefab;

[Header("Exclude Categories (optional)")]
public List<string> excludedCategoryNames;
public List<string> excludedCategoryNames_es;


    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    void Start()
    {
        Debug.Log("[StartupButtonHandler] Setting up button listener.");
        startButton.onClick.AddListener(() =>
        {
            Debug.Log("[StartupButtonHandler] Start button clicked!");
            startButton.interactable = false;
            StartCoroutine(Transition());
        });
    }

    IEnumerator Transition()
    {
        yield return StartCoroutine(FadeOut(titleCard));
        InstantiateCategoryButtons();
        yield return StartCoroutine(FadeIn(quizChoiceCard));
    }

    IEnumerator FadeOut(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        obj.SetActive(false);
    }

    IEnumerator FadeIn(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        obj.SetActive(true);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    void InstantiateCategoryButtons()
    {
        foreach (Transform child in categoryButtonParent)
        {
            Destroy(child.gameObject);
        }

        var cache = QuizDataCache.Instance;
        if (cache.CategoryItems == null || cache.CategoryItems.Count == 0)
        {
            Debug.LogWarning("[StartupButtonHandler] No CategoryItems loaded.");
            return;
        }

        var uniqueCategories = cache.CategoryItems
            .Select(q => q.Category1)
            .Distinct()
            .ToList();

        Button firstButton = null;

       foreach (var category in uniqueCategories)
{
    // Get a sample item for this category to access localized name
    var sample = cache.CategoryItems.FirstOrDefault(q => q.Category1 == category);
    string localizedCategory = LanguageController.Instance.IsSpanish()
        ? sample?.Category1_es ?? category
        : category;

    // Apply language-specific exclusion
    if (excludedCategoryNames.Contains(category) ||
        (sample != null && LanguageController.Instance.IsSpanish() &&
         excludedCategoryNames_es.Contains(sample.Category1_es)))
    {
        Debug.Log($"[StartupButtonHandler] Skipping excluded category: {localizedCategory}");
        continue;
    }

    GameObject buttonGO = Instantiate(categoryButtonPrefab, categoryButtonParent);
    TMP_Text label = buttonGO.GetComponentInChildren<TMP_Text>();

    if (label != null)
        label.text = localizedCategory;

    Button btn = buttonGO.GetComponent<Button>();
    if (btn != null)
    {
        string capturedCategory = category; // still use English key for data lookup
        btn.onClick.AddListener(() =>
        {
            Debug.Log($"[StartupButtonHandler] CLICK → Passing category: {capturedCategory}");
            PopulateQuizList(capturedCategory);
            categorySelector?.OnCategoryButtonClicked(btn);
        });

        if (categorySelector != null)
        {
            var border = buttonGO.transform.Find("Border");
            if (border != null)
                categorySelector.RegisterButton(btn, border.gameObject);
        }

        if (firstButton == null)
            firstButton = btn;
    }
}


        firstButton?.onClick.Invoke();
    }


void PopulateQuizList(string category)
{
    Debug.Log($"[StartupButtonHandler] Populating quiz list for category: {category}");

    // Clear existing buttons
    foreach (Transform child in quizListContainer)
    {
        Destroy(child.gameObject);
    }

    var cache = QuizDataCache.Instance;

    if (!cache.categoryToQuizNames.TryGetValue(category, out var quizzesInCategory))
    {
        Debug.LogWarning($"[StartupButtonHandler] No quiz names found for category: {category}");
        return;
    }

    foreach (var quizItem in quizzesInCategory)
    {
        int quizID = quizItem.QuizID;

        // Count number of entries for this quizID in quiz-details.json
        if (!cache.quizIDToThingIDs.TryGetValue(quizID.ToString(), out var thingIDs) || thingIDs.Count < 10)
        {
            Debug.Log($"[Filter] Skipping quizID {quizID} — only {thingIDs?.Count ?? 0} questions.");
            continue;
        }

        GameObject quizButtonGO = Instantiate(quizItemPrefab, quizListContainer);
        TMP_Text text = quizButtonGO.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = LanguageController.Instance.IsSpanish() ? quizItem.Name_es : quizItem.Name;
        }

        var selector = Object.FindFirstObjectByType<QuizButtonSelector>();
        selector?.RegisterButton(quizButtonGO.GetComponent<Button>());

        Button quizButton = quizButtonGO.GetComponent<Button>();
        if (quizButton != null)
        {
            quizButton.onClick.AddListener(() =>
            {
                Debug.Log("Quiz selected: " + quizID);
                var startBtn = Object.FindFirstObjectByType<StartQuizButtonHandler>();
                if (startBtn != null)
                {
                    startBtn.SetSelectedQuizID(quizID.ToString());
                }
            });
        }
    }
}



}
