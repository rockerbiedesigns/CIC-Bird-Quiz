using UnityEngine;
using UnityEngine.UI;

public class ButtonTester : MonoBehaviour
{
    public Button btn;

    void Start()
    {
        btn.onClick.AddListener(() => Debug.Log("[ButtonTester] Button was clicked"));
    }
}