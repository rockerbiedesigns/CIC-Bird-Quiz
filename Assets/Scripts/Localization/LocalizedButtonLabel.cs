using UnityEngine;
using TMPro;

public class LocalizedButtonLabel : MonoBehaviour
{
    public string englishText;
    public string spanishText;

    private TextMeshProUGUI textComponent;

    void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        UpdateLabel();
        LanguageToggleHandler.OnLanguageChanged += UpdateLabel;
    }

    void OnDisable()
    {
        LanguageToggleHandler.OnLanguageChanged -= UpdateLabel;
    }

    public void UpdateLabel()
    {
        if (textComponent == null) return;

        string lang = LocalizationManager.Instance.CurrentLanguage;
        textComponent.text = (lang == "es") ? spanishText : englishText;
    }
}
