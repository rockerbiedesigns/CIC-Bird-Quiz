// ✅ QuizDataCache.cs (updated to store QuizCategoryItem per category)
using System.Collections.Generic;
using UnityEngine;

public class QuizDataCache : MonoBehaviour
{
    public static QuizDataCache Instance { get; private set; }
    public List<QuizCategoryItem> CategoryItems = new();
    public Dictionary<string, QuizCategoryItem> quizIDToCategoryItem = new();
    public Dictionary<string, List<QuizCategoryItem>> categoryToQuizNames = new();

    public Dictionary<string, string> quizIDToName = new();
    public Dictionary<string, string> quizIDToName_es = new();
    public Dictionary<string, string> quizIDToCategory = new();
    public Dictionary<string, string> quizIDToCategory_es = new();
    public Dictionary<string, List<string>> quizIDToThingIDs = new();
    public Dictionary<string, string> thingIDToCommonName = new();
    public Dictionary<string, int> thingIDToMediaID = new();
    public Dictionary<int, string> mediaIDToPath = new();
    public Dictionary<string, string> thingIDToCommonNameID = new();
    public Dictionary<string, string> commonNameIDToCommonName = new();
    public Dictionary<string, string> commonNameIDToCommonName_es = new();
    public Dictionary<string, string> commonNameToCommonName_es = new();


    public Dictionary<string, List<string>> thingIDToSimilarThingIDs = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

public string GetCommonName(string thingID)
{
    if (!thingIDToCommonNameID.TryGetValue(thingID, out string commonNameID))
    {
        Debug.LogWarning($"[QuizDataCache] Missing CommonNameID for ThingID: {thingID}");
        return null;
    }

    if (!commonNameIDToCommonName.TryGetValue(commonNameID, out string commonName))
    {
        Debug.LogWarning($"[QuizDataCache] Missing CommonName for CommonNameID: {commonNameID}");
        return null;
    }

    return commonName;
}


    public List<string> GetSimilarCommonNames(string thingID)
    {
        List<string> similarCommonNames = new();
        if (thingIDToSimilarThingIDs.TryGetValue(thingID, out List<string> similarIDs))
        {
            foreach (string similarID in similarIDs)
            {
                string name = GetCommonName(similarID);
                if (!string.IsNullOrEmpty(name))
                    similarCommonNames.Add(name);
            }
        }
        return similarCommonNames;
    }

    public string GetMediaPath(string mediaID)
    {
        if (int.TryParse(mediaID, out int id) && mediaIDToPath.ContainsKey(id))
            return mediaIDToPath[id];

        Debug.LogWarning($"[QuizDataCache] Media path not found for MediaID: {mediaID}");
        return null;
    }

    public void ClearCache()
    {
        quizIDToThingIDs.Clear();
        thingIDToCommonName.Clear();
        thingIDToMediaID.Clear();
        thingIDToSimilarThingIDs.Clear();
        mediaIDToPath.Clear();
    }
}
