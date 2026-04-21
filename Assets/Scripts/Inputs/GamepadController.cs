using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamepadController : MonoBehaviour
{
    [SerializeField]
    private EventSystem _eventSystem;

    [SerializeField]
    private Button _useItemButt;

    public static GamepadController Instance;

    private void Awake()
    {
        if (Instance != this && Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void GoToButton(GameObject go)
    {
        _eventSystem.SetSelectedGameObject(go);
    }

}
