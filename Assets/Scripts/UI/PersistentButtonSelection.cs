using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PersistentButtonSelection : MonoBehaviour
{
    public Color selectedColor = new Color(0.6f, 0.8f, 1f, 1f); // Light blue
    public float selectedScale = 1.2f;
    public float normalScale = 1f;
    public Color normalColor = Color.white;

    [Header("Linked Object")]
    public GameObject linkedObject; // The object to fade in

    private Button thisButton;
    private Image thisImage;

    private static Button lastSelectedButton;
    private static Image lastSelectedImage;

    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisImage = GetComponent<Image>();

        if (thisButton != null)
            thisButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // Reset previously selected
        if (lastSelectedButton != null && lastSelectedButton != thisButton)
        {
            lastSelectedButton.transform.localScale = Vector3.one * normalScale;

            if (lastSelectedImage != null)
                lastSelectedImage.color = normalColor;
        }

        // Apply selection visuals
        transform.localScale = Vector3.one * selectedScale;

        if (thisImage != null)
            thisImage.color = selectedColor;

        lastSelectedButton = thisButton;
        lastSelectedImage = thisImage;
        // Make linked object fully visible (alpha = 1)
        if (linkedObject != null)
        {
            CanvasGroup cg = linkedObject.GetComponent<CanvasGroup>();
            Image img = linkedObject.GetComponent<Image>();

            if (cg != null)
            {
                cg.alpha = 1f; // works for any UI element with CanvasGroup
            }
            else if (img != null)
            {
                Color c = img.color;
                c.a = 1f; // make visible
                img.color = c;
            }
        }
    }

}
