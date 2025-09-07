using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerDisplayManager : MonoBehaviour
{
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    [SerializeField] public Button nextButton;

    private QuestionFlowController flowController;
    private Button selectedButton;
    private string correctAnswer;
    private bool hasCheckedAnswer = false;
    private string selectedAnswerText;

    void Start()
    {
        flowController = FindFirstObjectByType<QuestionFlowController>();
        if (flowController == null)
        {
            Debug.LogError("[AnswerDisplayManager] QuestionFlowController not found.");
        }
    }
public void RegisterSelectedButton(Button button, string internalAnswer)
{
    selectedButton = button;
    selectedAnswerText = internalAnswer;
    Debug.Log("[AnswerDisplayManager] Button registered: " + internalAnswer);
    if (nextButton != null)
    {
        nextButton.interactable = true;
    }
}


    public void SetCorrectAnswer(string answer)
    {
        correctAnswer = answer;
        Debug.Log("[AnswerDisplayManager] Correct answer set: " + correctAnswer);
    }

    public void SetFlowController(QuestionFlowController controller)
    {
        flowController = controller;
        if (flowController != null)
        {
            nextButton = flowController.nextButton;
        }
    }

    public void OnNextButtonPressed()
    {
        if (flowController == null || nextButton == null)
        {
            Debug.LogWarning("[AnswerDisplayManager] Flow controller or next button missing.");
            return;
        }

        if (!hasCheckedAnswer)
        {
            if (selectedButton == null)
            {
                Debug.Log("[AnswerDisplayManager] No answer selected.");
                return;
            }

            ShowFeedbackOnly();
            hasCheckedAnswer = true;

            string key = flowController.IsLastQuestion() ? "final_question" : "next_question";
            UpdateNextButtonText(LocalizationManager.Instance.Get(key));
        }
        else
        {
            ProceedToNextQuestion();
            hasCheckedAnswer = false;
            UpdateNextButtonText(LocalizationManager.Instance.Get("check_answer"));
        }
    }

   private void ShowFeedbackOnly()
{
    if (selectedButton == null) return;

    bool isCorrect = selectedAnswerText == correctAnswer;
    Debug.Log("[AnswerDisplayManager] Selected answer: " + selectedAnswerText);

    if (isCorrect)
    {
        SetButtonColor(selectedButton, correctColor);
        flowController.IncrementScore();
    }
    else
    {
        SetButtonColor(selectedButton, incorrectColor);

        // Find and highlight the correct button
        foreach (Transform child in flowController.commonNameListContainer)
        {
            Button b = child.GetComponent<Button>();
            TMP_Text txt = b.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                // Match the internal English name (correctAnswer) with the button's hidden binding
                string displayedText = txt.text;

                // Look up reverse-translation if current language is Spanish
                if (LanguageController.Instance.IsSpanish())
                {
                    foreach (var kvp in QuizDataCache.Instance.commonNameToCommonName_es)
                    {
                        if (kvp.Value == displayedText)
                        {
                            displayedText = kvp.Key; // Convert back to English for comparison
                            break;
                        }
                    }
                }

                if (displayedText == correctAnswer)
                {
                    SetButtonColor(b, correctColor);
                    break;
                }
            }
        }
    }
}

    private void ProceedToNextQuestion()
    {
        selectedButton = null;
        nextButton.interactable = false;

        flowController.AdvanceQuestionIndex();
        StartCoroutine(flowController.DisplayNextQuestion());
    }

    private void UpdateNextButtonText(string text)
    {
        TMP_Text label = nextButton.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = text;
    }

    private void SetButtonColor(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        cb.selectedColor = color;
        button.colors = cb;
    }
}
