using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecretQuitManager : MonoBehaviour
{
    [Header("Hidden Tap Trigger")]
    [SerializeField] private Button hiddenButton;
    [SerializeField] private int tapThreshold = 7;
    private int tapCount = 0;

    [Header("Quit Modal UI")]
    [SerializeField] private GameObject quitModalPanel;
    [SerializeField] private TMP_Text codeDisplayText;

    [Header("Number Buttons")]
    [SerializeField] private Button num0;
    [SerializeField] private Button num1;
    [SerializeField] private Button num2;
    [SerializeField] private Button num3;
    [SerializeField] private Button num4;
    [SerializeField] private Button num5;
    [SerializeField] private Button num6;
    [SerializeField] private Button num7;
    [SerializeField] private Button num8;
    [SerializeField] private Button num9;

    [Header("Control Buttons")]
    [SerializeField] private Button enterButton;
    [SerializeField] private Button clearButton;

    [Header("Code Settings")]
    [SerializeField] private string correctCode = "492";
    [SerializeField] private int maxCodeLength = 6;

    private string enteredCode = "";

    void Start()
    {
        hiddenButton.onClick.AddListener(OnHiddenButtonTapped);

        num0.onClick.AddListener(() => AppendDigit("0"));
        num1.onClick.AddListener(() => AppendDigit("1"));
        num2.onClick.AddListener(() => AppendDigit("2"));
        num3.onClick.AddListener(() => AppendDigit("3"));
        num4.onClick.AddListener(() => AppendDigit("4"));
        num5.onClick.AddListener(() => AppendDigit("5"));
        num6.onClick.AddListener(() => AppendDigit("6"));
        num7.onClick.AddListener(() => AppendDigit("7"));
        num8.onClick.AddListener(() => AppendDigit("8"));
        num9.onClick.AddListener(() => AppendDigit("9"));

        clearButton.onClick.AddListener(ClearCode);
        enterButton.onClick.AddListener(OnEnterPressed);

        quitModalPanel.SetActive(false);
        ClearCode();
    }

    void OnHiddenButtonTapped()
    {
        tapCount++;
        Debug.Log($"[SecretQuit] Tap {tapCount}/{tapThreshold}");

        if (tapCount >= tapThreshold)
        {
            ShowModal();
            tapCount = 0;
        }
    }

    void ShowModal()
    {
        ClearCode();
        quitModalPanel.SetActive(true);
        Debug.Log("[SecretQuit] Modal displayed.");
    }

    void AppendDigit(string digit)
    {
        if (enteredCode.Length >= maxCodeLength) return;

        enteredCode += digit;
        codeDisplayText.text = enteredCode;
        Debug.Log($"[SecretQuit] Button {digit} pressed. Current entry: {enteredCode}");
    }

    void ClearCode()
    {
        enteredCode = "";
        codeDisplayText.text = "";
        Debug.Log("[SecretQuit] Code cleared.");
    }

    void OnEnterPressed()
    {
        Debug.Log($"[SecretQuit] Enter pressed. Code entered: {enteredCode}");

        if (enteredCode == correctCode)
        {
            Debug.Log("[SecretQuit] Correct code entered. Quitting...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            Debug.Log("[SecretQuit] Incorrect code. Closing modal.");
            quitModalPanel.SetActive(false);
            ClearCode();
            tapCount = 0; // force re-tap to show modal again
        }
    }
}
