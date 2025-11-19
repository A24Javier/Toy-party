using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference touchDiceAction;

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
            return;
        }

        //Nos suscribimos
        touchDiceAction.action.performed += OnTouchDicePerformed;
        touchDiceAction.action.Enable();
    }

    private void OnDestroy()
    {
        //Desuscribirse para evitar múltiples llamadas si cambia de escena
        if (touchDiceAction != null)
            touchDiceAction.action.performed -= OnTouchDicePerformed;
    }

    private void OnTouchDicePerformed(InputAction.CallbackContext ctx)
    {
        //Ignorar toques si NO es turno del jugador
        if (!GameController.instance.IsPlayerRolling())
            return;

        spacePressed = true;

        // Restablecimiento automático
        StartCoroutine(ResetInputDelayed());
    }

    private System.Collections.IEnumerator ResetInputDelayed()
    {
        //Evita spam y multi-activaciones del input
        yield return new WaitForSeconds(0.1f);
        spacePressed = false;
    }


    public bool IsSpacebarTouched()
    {
        // Funciona igual que antes, pero más seguro
        if (spacePressed)
        {
            spacePressed = false;
            return true;
        }

        return false;
    }

    public void ResetSpace()
    {
        //Función para limpiar input al iniciar turno
        spacePressed = false;
    }
}
