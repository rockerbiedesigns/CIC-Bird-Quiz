using UnityEngine;

public class LanguageController : MonoBehaviour
{
    public static LanguageController Instance { get; private set; }

    private string currentLanguageCode = "en";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetLanguage(string langCode)
    {
        currentLanguageCode = langCode;
        Debug.Log($"[LanguageController] Language set to: {langCode}");
    }

    public string GetCurrentLanguage()
    {
        return currentLanguageCode;
    }

    public bool IsSpanish()
    {
        return currentLanguageCode == "es";
    }
}
