using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class QuizListScroller : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollStep = 0.05f;        // Amount scrolled per step
    [SerializeField] private float holdScrollDelay = 0.1f;    // Speed while holding
    [SerializeField] public Button upArrow;
    [SerializeField] public Button downArrow;
    private Coroutine scrollCoroutine;
    private CanvasGroup upArrowCanvasGroup;
    private CanvasGroup downArrowCanvasGroup;

    void Start()
    {
        upArrowCanvasGroup = upArrow.GetComponent<CanvasGroup>();
        downArrowCanvasGroup = downArrow.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        float pos = scrollRect.verticalNormalizedPosition;
        upArrowCanvasGroup.alpha = pos >= 0.99f ? 0.3f : 1f;
        downArrowCanvasGroup.alpha = pos <= 0.01f ? 0.3f : 1f;

        upArrow.interactable = pos < 0.99f;
        downArrow.interactable = pos > 0.01f;
    }

    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollStep);
    }

    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - scrollStep);
    }

    public void OnPointerDownScrollUp()
    {
        scrollCoroutine = StartCoroutine(ScrollContinuously(1));
    }

    public void OnPointerDownScrollDown()
    {
        scrollCoroutine = StartCoroutine(ScrollContinuously(-1));
    }

    public void OnPointerUp()
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null;
        }
    }

private IEnumerator ScrollContinuously(int direction)
{
    while (true)
    {
        float pos = scrollRect.verticalNormalizedPosition;
        float delta = direction * scrollStep;
        float newPos = Mathf.Clamp01(pos + delta);

        if (Mathf.Approximately(pos, newPos))
            yield break;

        scrollRect.verticalNormalizedPosition = newPos;
        yield return new WaitForSeconds(holdScrollDelay);
    }
}

    
}
