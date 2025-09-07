using UnityEngine;

public class ActivateChildrenOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
