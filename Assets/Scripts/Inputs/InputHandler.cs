using UnityEngine.InputSystem;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference touchDiceAction;
    [SerializeField] private InputActionReference returnAction;

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

        touchDiceAction.action.performed += OnTouchDicePerformed;
        touchDiceAction.action.Enable();

        returnAction.action.performed += OnReturnActionPressed;
        returnAction.action.Enable();
    }

    private void OnDestroy()
    {
        if (touchDiceAction != null)
            touchDiceAction.action.performed -= OnTouchDicePerformed;
    }

    #region Gestión de Input

    private void OnTouchDicePerformed(InputAction.CallbackContext ctx)
    {
        if (!GameController.instance.IsPlayerRolling())
            return;

        spacePressed = true;

        StartCoroutine(ResetInputDelayed());
    }

    private void OnReturnActionPressed(InputAction.CallbackContext ctx)
    {
        UIManager.instance.CloseItemPanel();
    }

    private System.Collections.IEnumerator ResetInputDelayed()
    {
        yield return new WaitForSeconds(0.25f);
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

    public void ResetSpace()
    {
        spacePressed = false;
    }

    #endregion
}
