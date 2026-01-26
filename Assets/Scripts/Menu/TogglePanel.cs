using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
