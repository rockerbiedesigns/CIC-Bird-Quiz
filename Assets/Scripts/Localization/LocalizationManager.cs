using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.Networking;

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    public string value;
}

[System.Serializable]
public class LocalizationData
{
    public List<LocalizationEntry> entries;

    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    public string CurrentLanguage { get; private set; } = "en";

    private Dictionary<string, string> localizedText = new();

    public static event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        string langCode = PlayerPrefs.GetString("lang", "en");
        LoadLanguage(langCode);
    }

    public void LoadLanguage(string langCode)
    {
        CurrentLanguage = langCode;

        string filePath = Path.Combine(Application.streamingAssetsPath, $"localization_{langCode}.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        string uri = filePath;
#else
        string uri = "file://" + filePath;
#endif

        StartCoroutine(LoadLocalizationFile(uri, langCode));
    }

    private IEnumerator LoadLocalizationFile(string uri, string langCode)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LocalizationManager] Failed to load localization file: {uri} — {www.error}");
                yield break;
            }

            string rawJson = www.downloadHandler.text;

            if (!rawJson.TrimStart().StartsWith("{\"entries\""))
            {
                rawJson = "{\"entries\":" + rawJson + "}";
            }

            LocalizationData data = JsonUtility.FromJson<LocalizationData>(rawJson);
            localizedText = data.ToDictionary();

            PlayerPrefs.SetString("lang", langCode);
            PlayerPrefs.Save();

            Debug.Log($"[LocalizationManager] Loaded {localizedText.Count} keys for {langCode}");

            OnLanguageChanged?.Invoke();
        }
    }

    public string Get(string key)
    {
        if (!localizedText.ContainsKey(key))
            Debug.LogWarning($"[LocalizationManager] Missing key: {key}");
        return localizedText.TryGetValue(key, out string value) ? value : $"[{key}]";
    }

    public void SetLanguage(string langCode)
    {
        LoadLanguage(langCode);
    }
}
