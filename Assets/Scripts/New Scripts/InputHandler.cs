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

        jumpDice.performed += ctx =>
        {
            // Solo registrar si el jugador PUEDE tirar el dado, INORANDO si no es tu turno
            if (GameController.instance.IsPlayerRolling())
            {
                spacePressed = true;
                StartCoroutine(SpaceBarControl());
            }
        };

    }
    //Resetea el espacio
    public void ResetSpace()
    {
        spacePressed = false;
    }


    private IEnumerator SpaceBarControl()
    {
        yield return new WaitForSeconds(0.2f);
        spacePressed = false;
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
