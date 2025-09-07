// InactivityTimeoutManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InactivityTimeoutManager : MonoBehaviour
{
    [Header("Monitored Panels")]
    [SerializeField] private List<GameObject> watchObjects;

    [Header("Prompt UI")]
    [SerializeField] private GameObject inactivityScreen;
    [SerializeField] private TMP_Text countdownText;

    [Header("Timeout Settings")]
    [SerializeField] private float inactivityDelay = 180f;
    [SerializeField] private float promptResponseTime = 30f;

    [Header("Dependencies")]
    [SerializeField] private RestartQuizButtonHandler restartHandler;
    [SerializeField] private GameObject startScreen;

    private float inactivityTimer;
    private float promptTimer;
    private bool promptActive = false;

    void Start()
    {
        inactivityTimer = inactivityDelay;
        inactivityScreen.SetActive(false);
        if (countdownText != null) countdownText.text = "";
    }

    void Update()
    {
        if (IsAnyPanelActive())
        {
            if (promptActive)
            {
                promptTimer -= Time.deltaTime;

                if (countdownText != null)
                    countdownText.text = $"Returning in: {Mathf.CeilToInt(promptTimer)}s";

                if (Input.anyKeyDown || Input.touchCount > 0)
                {
                    CancelPrompt();
                }
                else if (promptTimer <= 0f)
                {
                    // Deactivate watch objects
                    foreach (var obj in watchObjects)
                    {
                        if (obj != null)
                            obj.SetActive(false);
                    }

                    // Activate start screen
                    if (startScreen != null)
                        startScreen.SetActive(true);

                    // Call restart
                    if (restartHandler != null)
                        restartHandler.RestartQuiz();

                    // Cleanup prompt
                    inactivityScreen.SetActive(false);
                    promptActive = false;
                    inactivityTimer = inactivityDelay;
                    if (countdownText != null)
                        countdownText.text = "";
                }
            }
            else
            {
                inactivityTimer -= Time.deltaTime;
                if (Input.anyKeyDown || Input.touchCount > 0)
                {
                    inactivityTimer = inactivityDelay;
                }
                else if (inactivityTimer <= 0f)
                {
                    ShowPrompt();
                }
            }
        }
        else
        {
            inactivityTimer = inactivityDelay;
        }
    }

    private bool IsAnyPanelActive()
    {
        foreach (var obj in watchObjects)
        {
            if (obj != null && obj.activeInHierarchy)
                return true;
        }
        return false;
    }

    private void ShowPrompt()
    {
        inactivityScreen.SetActive(true);
        promptActive = true;
        promptTimer = promptResponseTime;
        if (countdownText != null)
            countdownText.text = $"Returning in: {Mathf.CeilToInt(promptTimer)}s";
    }

    private void CancelPrompt()
    {
        inactivityScreen.SetActive(false);
        promptActive = false;
        inactivityTimer = inactivityDelay;

        if (countdownText != null)
            countdownText.text = "";
    }
}
