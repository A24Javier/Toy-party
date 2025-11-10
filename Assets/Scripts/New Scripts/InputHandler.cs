using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public static InputHandler instance;

    [SerializeField] private InputActionAsset playerInputAction;
    private InputAction jumpDice;

    private bool spacePressed = false;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerInputAction.Enable();
        jumpDice = playerInputAction.FindActionMap("Player").FindAction("TouchDice");
        jumpDice.Enable();

        jumpDice.performed += ctx => spacePressed = true;
    }

    void Start()
    {
        
    }

    public bool IsSpacebarTouched()
    {
        if (spacePressed)
        {
            spacePressed = false;
            return true;
        }
        
        return false;
    }
}
