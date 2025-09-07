using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButtonSelector : MonoBehaviour
{
    public Color selectedBorderColor = Color.yellow;
    public Color defaultBorderColor = Color.white;
    public float selectedScale = 1.1f;
    public float defaultScale = 1f;

    private Button currentlySelected;
    private Dictionary<Button, GameObject> buttonToBorder = new Dictionary<Button, GameObject>();

    public void RegisterButton(Button btn, GameObject border)
    {
        Debug.Log($"[CategoryButtonSelector] Registering {btn.name} with border {border?.name}");

        buttonToBorder[btn] = border;
        ResetVisuals(btn);
    }

    public void OnCategoryButtonClicked(Button clickedButton)
    {
        Debug.Log($"[CategoryButtonSelector] Button clicked: {clickedButton.name}");

        if (currentlySelected != null)
        {
            ResetVisuals(currentlySelected);
        }

        currentlySelected = clickedButton;
        ApplySelectedVisuals(clickedButton);
    }

    private void ApplySelectedVisuals(Button btn)
    {
        btn.transform.localScale = Vector3.one * selectedScale;

        if (buttonToBorder.TryGetValue(btn, out GameObject border))
        {
            Image img = border.GetComponent<Image>();
            if (img != null)
            {
                img.color = selectedBorderColor;
            }
        }
    }

    private void ResetVisuals(Button btn)
    {
        btn.transform.localScale = Vector3.one * defaultScale;

        if (buttonToBorder.TryGetValue(btn, out GameObject border))
        {
            Image img = border.GetComponent<Image>();
            if (img != null)
            {
                img.color = defaultBorderColor;
            }
        }
    }
}
