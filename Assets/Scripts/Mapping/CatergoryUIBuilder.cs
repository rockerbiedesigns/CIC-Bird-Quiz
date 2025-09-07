using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryUIBuilder
{
    private Transform contentParent;
    private GameObject categoryContainerPrefab;
    private Button quizItemPrefab;
    private List<CategoryPositionMapping> manualPositions;
    private List<CanvasGroup> fadables;
    private System.Action<string> quizClickCallback;

    public CategoryUIBuilder(
        Transform contentParent,
        GameObject categoryContainerPrefab,
        Button quizItemPrefab,
        List<CategoryPositionMapping> manualPositions,
        List<CanvasGroup> fadables,
        System.Action<string> quizClickCallback)
    {
        this.contentParent = contentParent;
        this.categoryContainerPrefab = categoryContainerPrefab;
        this.quizItemPrefab = quizItemPrefab;
        this.manualPositions = manualPositions;
        this.fadables = fadables;
        this.quizClickCallback = quizClickCallback;
    }

    public void CreateCategory(string categoryName, List<string> quizNames)
    {
        GameObject categoryGO = GameObject.Instantiate(categoryContainerPrefab, contentParent);
        CanvasGroup cg = categoryGO.GetComponent<CanvasGroup>();
        if (cg != null) fadables.Add(cg);

        CategoryPositionMapping customPos = manualPositions.Find(p => p.categoryName == categoryName);
        if (customPos != null)
        {
            RectTransform rt = categoryGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = customPos.anchoredPosition;
            }
        }

        Button categoryButton = categoryGO.GetComponentInChildren<Button>();
        TMP_Text buttonText = categoryButton.GetComponentInChildren<TMP_Text>();
        Transform quizListContainer = categoryGO.transform.Find("QuizList");

        if (buttonText != null) buttonText.text = categoryName;
        if (quizListContainer != null) quizListContainer.gameObject.SetActive(false);

        foreach (var quizName in quizNames)
        {
            Button quizButton = GameObject.Instantiate(quizItemPrefab, quizListContainer);
            quizButton.transform.SetAsLastSibling();

            TMP_Text quizText = quizButton.GetComponentInChildren<TMP_Text>();
            if (quizText != null) quizText.text = quizName;

            CanvasGroup qcg = quizButton.GetComponent<CanvasGroup>();
            if (qcg != null) fadables.Add(qcg);

            string capturedName = quizName;
            quizButton.onClick.AddListener(() => quizClickCallback.Invoke(capturedName));
        }

        categoryButton.onClick.AddListener(() =>
        {
            if (quizListContainer != null)
            {
                bool isActive = quizListContainer.gameObject.activeSelf;
                quizListContainer.gameObject.SetActive(!isActive);
            }
        });
    }
}
