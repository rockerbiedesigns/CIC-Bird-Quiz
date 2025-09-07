// Attach this to a blank GameObject in a new scene
using UnityEngine;
using UnityEngine.UI;

public class Quit : MonoBehaviour
{    public Button button1;
    public Button button2;

    private bool b1Pressed = false;
    private bool b2Pressed = false;

    void Start()
    {
        // Button listeners
        button1.onClick.AddListener(() => { b1Pressed = true; CheckAllPressed(); });
        button2.onClick.AddListener(() => { b2Pressed = true; CheckAllPressed(); });
    }

    void CheckAllPressed()
    {
        if (b1Pressed && b2Pressed)
        {
            Debug.Log("✅ Both buttons pressed. Quitting application.");
            Application.Quit();
        }
        if (b1Pressed && !b2Pressed)
        {
            Debug.Log("✅ One button pressed.");
            Application.Quit();
        }
                if (!b1Pressed && b2Pressed)
        {
            Debug.Log("✅ One button pressed.");
            Application.Quit();
        }
    }
}