using UnityEngine;
using UnityEngine.UI;

public class LanguageInitializer : MonoBehaviour
{
    [Header("Language Buttons")]
    [SerializeField] private Button englishButton;

    private void Start()
    {
        if (englishButton != null)
        {
            Debug.Log("[LanguageInitializer] Setting default language to English...");
            englishButton.onClick.Invoke(); // Simulate click
        }
        else
        {
            Debug.LogWarning("[LanguageInitializer] English button reference is missing.");
        }
    }
}
