using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BorderVisualSettings
{
    public string tag;
    public Color defaultColor = new Color(0.6f, 0.3f, 0.1f);
    public Color selectedColor = Color.yellow;
    public float selectedScale = 1.05f;
}

public class QuizButtonSelector : MonoBehaviour
{
    [SerializeField] private List<BorderVisualSettings> visualSettingsList;

    private Dictionary<Button, GameObject> buttonToBorder = new Dictionary<Button, GameObject>();
    private Dictionary<Button, BorderVisualSettings> buttonToSettings = new Dictionary<Button, BorderVisualSettings>();
    private Dictionary<Button, TMP_Text> buttonToText = new Dictionary<Button, TMP_Text>();
    private Dictionary<Button, string> originalTextContent = new Dictionary<Button, string>();

    private Button currentSelectedButton;

    public void RegisterButton(Button btn)
    {
        // Locate border by tag
        foreach (var settings in visualSettingsList)
        {
            GameObject borderGO = FindChildWithTag(btn.gameObject, settings.tag);
            if (borderGO != null)
            {
                buttonToBorder[btn] = borderGO;
                buttonToSettings[btn] = settings;

                TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    buttonToText[btn] = text;
                    originalTextContent[btn] = text.text;
                }

                btn.onClick.AddListener(() => OnQuizButtonClicked(btn));
                return;
            }
        }

        Debug.LogWarning($"No matching border tag found for button: {btn.name}");
    }

    private GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag(tag))
                return child.gameObject;
        }
        return null;
    }

    private void OnQuizButtonClicked(Button clickedButton)
    {
        if (currentSelectedButton == clickedButton) return;

        if (currentSelectedButton != null)
        {
            ResetVisuals(currentSelectedButton);
        }

        currentSelectedButton = clickedButton;
        ApplySelectedVisuals(clickedButton);
    }

    private void ApplySelectedVisuals(Button btn)
    {
        if (!buttonToBorder.TryGetValue(btn, out GameObject border) ||
            !buttonToSettings.TryGetValue(btn, out BorderVisualSettings settings))
        {
            Debug.LogWarning("Missing border or visual settings for selected button.");
            return;
        }

        border.GetComponent<Image>().color = settings.selectedColor;
        btn.transform.localScale = Vector3.one * settings.selectedScale;

        if (buttonToText.TryGetValue(btn, out TMP_Text text) &&
            originalTextContent.TryGetValue(btn, out string originalText))
        {
            text.text = $"<u>{originalText}</u>";
        }
    }

    private void ResetVisuals(Button btn)
    {
        if (!buttonToBorder.TryGetValue(btn, out GameObject border) ||
            !buttonToSettings.TryGetValue(btn, out BorderVisualSettings settings))
        {
            Debug.LogWarning("Missing border or visual settings for previously selected button.");
            return;
        }

        border.GetComponent<Image>().color = settings.defaultColor;
        btn.transform.localScale = Vector3.one;

        if (buttonToText.TryGetValue(btn, out TMP_Text text) &&
            originalTextContent.TryGetValue(btn, out string originalText))
        {
            text.text = originalText;
        }
    }
}
