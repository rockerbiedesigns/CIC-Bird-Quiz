
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartQuizButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject quizChoiceCard;
    [SerializeField] private CanvasGroup quizChoiceCanvasGroup;
    [SerializeField] private GameObject questionScreen;
    [SerializeField] private Button startButton;

    private string selectedQuizID;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartQuizClicked);
        }
    }

    public void SetSelectedQuizID(string quizID)
    {
        selectedQuizID = quizID;
        Debug.Log($"[StartQuizButtonHandler] Selected QuizID: {selectedQuizID}");
    }

    private void OnStartQuizClicked()
    {
        if (string.IsNullOrEmpty(selectedQuizID))
        {
            Debug.LogWarning("[StartQuizButtonHandler] No quiz selected. Cannot start.");
            return;
        }

        Debug.Log("[StartQuizButtonHandler] Starting quiz sequence...");

        // Disable all interactive elements in the quizChoiceCard
        Button[] buttons = quizChoiceCard.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = false;
        }

        // Fade out the quizChoiceCard
        if (quizChoiceCanvasGroup != null)
        {
            StartCoroutine(FadeAndSwitch());

        }
        else
        {
            quizChoiceCard.SetActive(false);
            ActivateQuestionScreen();
        }
    }

private void ActivateQuestionScreen()
{
    if (questionScreen != null)
    {
        questionScreen.SetActive(true);

        var screen = questionScreen.GetComponent<QuestionScreen>();
        if (screen != null)
        {
            Debug.Log($"[StartQuizButtonHandler] Starting quiz from QuestionScreen with ID: {selectedQuizID}");
            screen.StartQuiz(selectedQuizID);
        }
        else
        {
            Debug.LogWarning("[StartQuizButtonHandler] QuestionScreen component not found.");
        }
    }
}


    private IEnumerator FadeAndSwitch()
    {
        float duration = 0.5f;
        float time = 0f;
        float startAlpha = quizChoiceCanvasGroup.alpha;

        while (time < duration)
        {
            quizChoiceCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        quizChoiceCanvasGroup.alpha = 0f;
        quizChoiceCard.SetActive(false);
        ActivateQuestionScreen();
    }

private IEnumerator DelayedLoadQuiz(QuestionFlowController controller)
{
    yield return null; // wait one frame
    Debug.Log($"[StartQuizButtonHandler] Delayed LoadQuiz: {selectedQuizID}");
    controller.LoadQuiz(selectedQuizID);
}


}
