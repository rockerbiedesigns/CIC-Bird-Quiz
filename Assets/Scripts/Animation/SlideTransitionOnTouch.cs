using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class SlideTransitionOnTouch : MonoBehaviour
{
    public GameObject currentObject;
    public GameObject nextObject;
    public GameObject quizButtons;
    public float slideDuration = 1f;
    public float offscreenOffset = 1920f;

    private bool isTransitioning = false;
    private Vector3 nextObjectTargetPosition;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += OnFingerDown;

        if (nextObject != null)
        {
            nextObjectTargetPosition = nextObject.transform.localPosition;
            nextObject.transform.localPosition = nextObjectTargetPosition + new Vector3(offscreenOffset, 0, 0);
            nextObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
        EnhancedTouchSupport.Disable();
    }

    void OnFingerDown(Finger finger)
    {
        if (!isTransitioning)
        {
            StartCoroutine(SlideTransition());
        }
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(SlideTransition());
        }
    }

    System.Collections.IEnumerator SlideTransition()
    {
        isTransitioning = true;

        Vector3 startPosCurrent = currentObject.transform.localPosition;
        Vector3 endPosCurrent = startPosCurrent + new Vector3(offscreenOffset, 0, 0);

        Vector3 startPosNext = nextObject.transform.localPosition;
        Vector3 endPosNext = nextObjectTargetPosition;

        nextObject.SetActive(true);
        quizButtons.SetActive(true);

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            currentObject.transform.localPosition = Vector3.Lerp(startPosCurrent, endPosCurrent, t);
            nextObject.transform.localPosition = Vector3.Lerp(startPosNext, endPosNext, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentObject.transform.localPosition = endPosCurrent;
        nextObject.transform.localPosition = endPosNext;
        currentObject.SetActive(false);

        // ✅ Start feather animation after slide transition
        FeatherMoveToFinalPosition featherScript = quizButtons.GetComponent<FeatherMoveToFinalPosition>();
        if (featherScript != null)
        {
            Debug.Log("SlideTransition: Starting feather slide...");
            yield return StartCoroutine(featherScript.PlayFeatherSlide());
        }
        else
        {
            Debug.LogError("SlideTransition: FeatherMoveToFinalPosition script not found on quizButtons.");
        }
    }
}
