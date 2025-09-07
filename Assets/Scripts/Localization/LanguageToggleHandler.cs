using UnityEngine;
using System;

public class LanguageToggleHandler : MonoBehaviour
{
    [Tooltip("Use 'en' for English, 'es' for Spanish.")]
    [SerializeField] private string spanishCode = "es";
    [SerializeField] private string englishCode = "en";

    public static event Action OnLanguageChanged;

    private void Start()
    {
        // Optional: auto-load language from PlayerPrefs
        string savedLang = PlayerPrefs.GetString("lang", englishCode);
        SetLanguage(savedLang);
    }

    public void SetLanguage(string langCode)
    {
        if (string.IsNullOrEmpty(langCode))
        {
            langCode = "en"; // fallback to English
            Debug.LogWarning("[LanguageToggleHandler] Empty langCode, defaulting to 'en'");
        }

        PlayerPrefs.SetString("lang", langCode);
        PlayerPrefs.Save();
        LocalizationManager.Instance.LoadLanguage(langCode);
        OnLanguageChanged?.Invoke();
        Debug.Log("[LanguageToggleHandler] langCode = " + langCode);
    }

public void SwitchToEnglish()
{
    SetLanguage("en");
}

public void SwitchToSpanish()
{
    SetLanguage("es");
}


}
