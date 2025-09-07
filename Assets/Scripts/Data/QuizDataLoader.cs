using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

using UnityEngine;

[System.Serializable]
public class QuizSheetWrapper
{
    public List<QuizCategoryItem> Sheet1;
}

[System.Serializable]
public class QuizDetailsItem
{
    public string QuizID;
    public string ThingID;
}

[System.Serializable]
public class QuizDetailsWrapper
{
    public List<QuizDetailsItem> Details; // ✅ matches the top-level key in quiz-details.json
}

[System.Serializable]
public class MediaThingLink
{
    public string ThingID;
    public int MediaID;
}

[System.Serializable]
public class MediaThingWrapper
{
    public List<MediaThingLink> Links;
}



[System.Serializable]
public class MediaPathItem
{
    public string MediaID;
    public string Path;
}

[System.Serializable]
public class MediaPathWrapper
{
    public List<MediaPathItem> Sheet1;
}
[System.Serializable]
public class CommonNameItem
{
    public string ThingID;
    public string CommonNameID;
    public string CommonName;
    public string CommonName_es;
}

[System.Serializable]
public class CommonNameWrapper
{
    public List<CommonNameItem> Sheet1; // Adjust if the root is different
}

[System.Serializable]
public class ClassificationItem
{
    public string ThingID;
    public string CommonNameID;
}

[System.Serializable]
public class ClassificationWrapper
{
    public List<ClassificationItem> Sheet1;
}



public class QuizDataLoader : MonoBehaviour
{
    [SerializeField] private string jsonFileName = "excel-to-json_translated.json";

    // Instead of Awake
    private void Start()
    {
        StartCoroutine(LoadCategoryData());
        StartCoroutine(LoadQuizDetailsData());
        StartCoroutine(LoadCommonNames());
        StartCoroutine(LoadMediaThingsData());
        StartCoroutine(LoadMediaPaths());
        StartCoroutine(LoadClassifications());
        Debug.Log($"[QuizDataLoader] Loaded {QuizDataCache.Instance.quizIDToThingIDs.Count} quizID → ThingID entries.");

    }

    IEnumerator LoadCategoryData()
    {
        Debug.Log("[QuizDataLoader] LoadCategoryData() started.");
        yield return StartCoroutine(LoadJson(jsonFileName, (string json) =>
        {
            QuizSheetWrapper loadedWrapper = JsonUtility.FromJson<QuizSheetWrapper>(json);
            if (loadedWrapper?.Sheet1 == null || loadedWrapper.Sheet1.Count == 0)
            {
                Debug.LogWarning("[QuizDataLoader] No data found in JSON.");
                return;
            }

            var cache = QuizDataCache.Instance;
            cache.CategoryItems = loadedWrapper.Sheet1;

            foreach (var item in loadedWrapper.Sheet1)
            {
                string quizID = item.QuizID.ToString();

                cache.quizIDToCategory[quizID] = item.Category1;
                cache.quizIDToCategory_es[quizID] = item.Category1_es;

                if (!cache.categoryToQuizNames.ContainsKey(item.Category1))
                    cache.categoryToQuizNames[item.Category1] = new List<QuizCategoryItem>();

                cache.categoryToQuizNames[item.Category1].Add(item);
                cache.quizIDToCategoryItem[quizID] = item;
                cache.quizIDToName[quizID] = item.Name;
                cache.quizIDToName_es[quizID] = item.Name_es;
            }

            Debug.Log($"[QuizDataLoader] Loaded {loadedWrapper.Sheet1.Count} quiz items.");
        }));
    }

    IEnumerator LoadQuizDetailsData()
    {
        yield return StartCoroutine(LoadJson("quiz-details.json", (string json) =>
        {
            QuizDetailsWrapper wrapper = JsonUtility.FromJson<QuizDetailsWrapper>(json);

            foreach (var item in wrapper.Details)
            {
                string quizID = item.QuizID;
                string thingID = item.ThingID;

                if (!QuizDataCache.Instance.quizIDToThingIDs.ContainsKey(quizID))
                    QuizDataCache.Instance.quizIDToThingIDs[quizID] = new List<string>();

                QuizDataCache.Instance.quizIDToThingIDs[quizID].Add(thingID);
            }

            Debug.Log($"[QuizDataLoader] Loaded ThingIDs for {QuizDataCache.Instance.quizIDToThingIDs.Count} quizzes.");
        }));
    }

    IEnumerator LoadCommonNames()
    {
        yield return StartCoroutine(LoadJson("common-name_translated.json", (string json) =>
        {
            CommonNameWrapper wrapper = JsonUtility.FromJson<CommonNameWrapper>(json);

            if (wrapper?.Sheet1 == null)
            {
                Debug.LogWarning("[QuizDataLoader] No entries found in common-name_translated.json");
                return;
            }

            foreach (var item in wrapper.Sheet1)
            {
                if (!string.IsNullOrEmpty(item.CommonNameID))
                {
                    if (!string.IsNullOrEmpty(item.CommonName))
                        QuizDataCache.Instance.commonNameIDToCommonName[item.CommonNameID] = item.CommonName;

                    if (!string.IsNullOrEmpty(item.CommonName_es))
                        QuizDataCache.Instance.commonNameIDToCommonName_es[item.CommonNameID] = item.CommonName_es;
                }

                if (!string.IsNullOrEmpty(item.CommonName) && !string.IsNullOrEmpty(item.CommonName_es))
                    QuizDataCache.Instance.commonNameToCommonName_es[item.CommonName] = item.CommonName_es;
            }

            Debug.Log($"[QuizDataLoader] Loaded {QuizDataCache.Instance.commonNameIDToCommonName.Count} CommonName entries.");
        }));
    }

    IEnumerator LoadMediaThingsData()
    {
        yield return StartCoroutine(LoadJson("media-things.json", (string json) =>
        {
            MediaThingWrapper wrapper = JsonUtility.FromJson<MediaThingWrapper>(json);

            foreach (var link in wrapper.Links)
            {
                if (!QuizDataCache.Instance.thingIDToMediaID.ContainsKey(link.ThingID))
                    QuizDataCache.Instance.thingIDToMediaID[link.ThingID] = link.MediaID;
            }

            Debug.Log($"[QuizDataLoader] Loaded MediaID links for {QuizDataCache.Instance.thingIDToMediaID.Count} ThingIDs.");
        }));
    }

    IEnumerator LoadMediaPaths()
    {
        yield return StartCoroutine(LoadJson("media.json", (string json) =>
        {
            MediaPathWrapper wrapper = JsonUtility.FromJson<MediaPathWrapper>(json);

            foreach (var item in wrapper.Sheet1)
            {
                if (int.TryParse(item.MediaID, out int id))
                    QuizDataCache.Instance.mediaIDToPath[id] = item.Path;
            }

            Debug.Log($"[QuizDataLoader] Loaded {QuizDataCache.Instance.mediaIDToPath.Count} media paths.");
        }));
    }

    IEnumerator LoadClassifications()
    {
        yield return StartCoroutine(LoadJson("classifications.json", (string json) =>
        {
            ClassificationWrapper wrapper = JsonUtility.FromJson<ClassificationWrapper>(json);

            foreach (var item in wrapper.Sheet1)
                QuizDataCache.Instance.thingIDToCommonNameID[item.ThingID] = item.CommonNameID;

            Debug.Log($"[QuizDataLoader] Loaded {wrapper.Sheet1.Count} classification entries.");
        }));
    }

    IEnumerator LoadJson(string filename, System.Action<string> onSuccess)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
#if UNITY_ANDROID && !UNITY_EDITOR
        string uri = path;
#else
        string uri = "file://" + path;
#endif
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[QuizDataLoader] Failed to load {filename}: {www.error}");
            }
        }
    }
}