using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Toggle()
    {
        if (panel == null) return;

        panel.SetActive(!panel.activeSelf);
    }
}