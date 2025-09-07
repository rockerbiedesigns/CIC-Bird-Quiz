using UnityEngine;

public class ImmersiveModeEnforcer : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        EnableImmersiveMode();
#endif
    }

    void EnableImmersiveMode()
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
                AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView");

                int flags = 
                    0x00001000 | // SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                    0x00000002 | // SYSTEM_UI_FLAG_HIDE_NAVIGATION
                    0x00000004 | // SYSTEM_UI_FLAG_FULLSCREEN
                    0x00000100 | // SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                    0x00000200;  // SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN

                decorView.Call("setSystemUiVisibility", flags);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Immersive mode failed: " + e.Message);
        }
    }
}