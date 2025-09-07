// Attach this to a blank GameObject in a new scene
using UnityEngine;
using UnityEngine.UI;

public class AndroidUIButtonTest : MonoBehaviour

{    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        HideAndroidUI();
        StartCoroutine(WatchForSystemUIReveal());
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    void HideAndroidUI()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var window = activity.Call<AndroidJavaObject>("getWindow"))
        using (var view = window.Call<AndroidJavaObject>("getDecorView"))
        {
            int flags =
                0x00000400 | // View.SYSTEM_UI_FLAG_FULLSCREEN
                0x00000200 | // View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                0x00001000 | // View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                0x00000100;  // View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION

            view.Call("setSystemUiVisibility", flags);
        }
    }

    System.Collections.IEnumerator WatchForSystemUIReveal()
    {
        while (true)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                yield return new WaitForSeconds(1f); // wait a moment
                HideAndroidUI(); // re-hide if user tapped
            }
            yield return null;
        }
    }
#endif
}
