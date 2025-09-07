
using UnityEngine;
using TMPro;
using System.Collections;

public class DisplayFinalScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public QuestionFlowController flowController;

    void OnEnable()
    {
        Debug.Log("[DisplayFinalScoreUI] OnEnable fired");
        LocalizationManager.OnLanguageChanged += RefreshScoreDisplay;
        RefreshScoreDisplay(); // trigger right away
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= RefreshScoreDisplay;
    }

    private void RefreshScoreDisplay()
    {
        StartCoroutine(WaitForLocalizationAndDisplay());
    }

private IEnumerator WaitForLocalizationAndDisplay()
{
    while (LocalizationManager.Instance == null)
    {
        Debug.Log("[DisplayFinalScoreUI] Waiting for LocalizationManager...");
        yield return null;
    }

    if (flowController == null || scoreText == null)
    {
        Debug.LogWarning("[DisplayFinalScoreUI] MISSING REFS. flowController: " + (flowController != null) + ", scoreText: " + (scoreText != null));
        yield break;
    }

    int realScore = flowController.Score;
    int totalQuestions = flowController.TotalQuestions;

    string template = LocalizationManager.Instance.Get("final_score");
    Debug.Log($"[DisplayFinalScoreUI] Using template: {template}");

    scoreText.text = string.Format(template, realScore, totalQuestions);
}



}


