using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleConfig : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Header("Eventos")]
    public UnityEvent FristClick;
    public UnityEvent SecondClick;
    public Button config;
    public Button pause;
    public Button otro;


    private bool isOpen = false;

    public void Toggle()
    {
        Navigation nav = config.navigation;

        isOpen = !isOpen;

        panel.SetActive(isOpen);

        if (isOpen)
        {
            FristClick?.Invoke();
            nav.selectOnDown = otro;
        }
        else
        {
            SecondClick?.Invoke();
            nav.selectOnDown = pause;
            
        }
        config.navigation = nav;   
    }
}