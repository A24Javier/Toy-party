using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceDetector : MonoBehaviour
{
    [SerializeField] private TMP_Text deviceText;

    [Header("Tuning")]
    [SerializeField] private float switchCooldown = 0.25f; // evita parpadeo

    private enum Scheme { KeyboardMouse, Gamepad }
    private Scheme currentScheme = Scheme.KeyboardMouse;

    private float lastSwitchTime;

    private void Start()
    {
        // Estado inicial: si hay mando conectado, puedes decidir arrancar en mando o teclado
        currentScheme = Gamepad.current != null ? Scheme.Gamepad : Scheme.KeyboardMouse;
        ApplyText();
    }

    private void Update()
    {
        if (Time.unscaledTime - lastSwitchTime < switchCooldown)
            return;

        // 1) Si el gamepad ha hecho input real -> cambiar a Gamepad
        if (GamepadDidMeaningfulInput())
        {
            SwitchTo(Scheme.Gamepad);
            return;
        }

        // 2) Si teclado ha hecho input real -> cambiar a KeyboardMouse
        //    (mouse lo ignoramos porque suele dar falsos positivos)
        if (KeyboardDidMeaningfulInput())
        {
            SwitchTo(Scheme.KeyboardMouse);
            return;
        }
    }

    private void SwitchTo(Scheme newScheme)
    {
        if (newScheme == currentScheme) return;

        currentScheme = newScheme;
        lastSwitchTime = Time.unscaledTime;
        ApplyText();
    }

    private void ApplyText()
    {
        deviceText.text = (currentScheme == Scheme.Gamepad)
            ? "USANDO: GAMEPAD "
            : "USANDO: TECLADO ";
    }

    private bool KeyboardDidMeaningfulInput()
    {
        if (Keyboard.current == null) return false;

        // anyKey funciona muy bien para detectar que realmente se pulsó algo
        return Keyboard.current.anyKey.wasPressedThisFrame;
    }

    private bool GamepadDidMeaningfulInput()
    {
        if (Gamepad.current == null) return false;

        var gp = Gamepad.current;

        // Botones
        if (gp.buttonSouth.wasPressedThisFrame) return true;
        if (gp.buttonNorth.wasPressedThisFrame) return true;
        if (gp.buttonWest.wasPressedThisFrame) return true;
        if (gp.buttonEast.wasPressedThisFrame) return true;
        if (gp.startButton.wasPressedThisFrame) return true;
        if (gp.selectButton.wasPressedThisFrame) return true;

        // Dpad
        if (gp.dpad.up.wasPressedThisFrame || gp.dpad.down.wasPressedThisFrame ||
            gp.dpad.left.wasPressedThisFrame || gp.dpad.right.wasPressedThisFrame) return true;

        // Sticks (umbral para evitar ruido)
        if (gp.leftStick.ReadValue().sqrMagnitude > 0.20f * 0.20f) return true;
        if (gp.rightStick.ReadValue().sqrMagnitude > 0.20f * 0.20f) return true;

        // Triggers (umbral)
        if (gp.leftTrigger.ReadValue() > 0.25f) return true;
        if (gp.rightTrigger.ReadValue() > 0.25f) return true;

        return false;
    }

    // Esto lo vamos a usar en el siguiente paso para decidir qué bindings mostrar
    public bool IsUsingGamepad() => currentScheme == Scheme.Gamepad;
}