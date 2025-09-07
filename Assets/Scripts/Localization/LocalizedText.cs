using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string localizationKey;

    void Start()
    {
        TMP_Text text = GetComponent<TMP_Text>();

        if (string.IsNullOrWhiteSpace(localizationKey))
        {
            Debug.LogWarning($"[LocalizedText] Missing or empty localizationKey on: {gameObject.name}");
            return;
        }

        // If LocalizationManager is not yet ready, wait
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("[LocalizedText] LocalizationManager not ready.");
            return;
        }

        // If key is still missing, wait for event
        string textValue = LocalizationManager.Instance.Get(localizationKey);
        if (textValue.StartsWith("["))
        {
            LocalizationManager.OnLanguageChanged += UpdateText;
            return;
        }

        text.text = LocalizationManager.Instance.Get(localizationKey);
        LocalizationManager.OnLanguageChanged += UpdateText;
    }

    void OnDestroy()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = LocalizationManager.Instance.Get(localizationKey);
    }
}