using UnityEngine;
using UnityEngine.EventSystems;

public class InputDebugger : MonoBehaviour
{
    void Update()
    {
        // Touch test
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log($"[InputDebugger] Touch detected at: {touch.position}");
        }

        // Mouse click test (works in Play Mode even if Android is active platform)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[InputDebugger] Mouse click detected at: {Input.mousePosition}");
        }
    }
}
