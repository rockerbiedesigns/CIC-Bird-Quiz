using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class QuizImageLoader : MonoBehaviour
{
    public static IEnumerator LoadImage(RawImage rawImage, string imagePath)
{
    if (string.IsNullOrEmpty(imagePath))
    {
        Debug.LogError("[QuizImageLoader] imagePath is null or empty.");
        yield break;
    }

    string normalizedPath = imagePath.Replace("\\", "/");
    string fullPath = Path.Combine(Application.streamingAssetsPath, normalizedPath);

#if UNITY_ANDROID && !UNITY_EDITOR
    string url = fullPath; // no file:// needed on Android
#else
    string url = "file://" + fullPath;
#endif

    Debug.Log($"[QuizImageLoader] Loading image from: {url}");

    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[QuizImageLoader] Failed to load image. Error: {www.error}");
        }
        else
        {
            rawImage.texture = DownloadHandlerTexture.GetContent(www);
            rawImage.SetNativeSize();
        }
    }
}


}
