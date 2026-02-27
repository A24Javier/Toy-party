using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardMoveCompositeRowUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DeviceDetector deviceDetector;
    [SerializeField] private RebindSaveLoad saveLoad;
    [SerializeField] private InputActionReference actionRef; // Player/Move

    [Header("Composite Part")]
    [Tooltip("Up / Down / Left / Right (como lo ves en el editor)")]
    [SerializeField] private string compositePartName = "Up";

    [Header("UI")]
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private TMP_Text rebindButtonText; // opcional

    [Header("Rebind")]
    [SerializeField] private float matchWaitSeconds = 0.1f;
    [SerializeField] private string cancelPath = "<Keyboard>/escape";

    private InputActionRebindingExtensions.RebindingOperation op;
    private string lastShown = "";

    private void OnEnable()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.RemoveAllListeners();
            rebindButton.onClick.AddListener(StartRebindKeyboard);
        }

        Refresh();
    }

    private void OnDisable() => DisposeOp();

    private void Update() => Refresh();

    private void Refresh()
    {
        if (actionRef == null || actionRef.action == null || bindingText == null)
            return;

        bool usingGamepad = deviceDetector != null && deviceDetector.IsUsingGamepad();

        // ✅ SIEMPRE mostramos la tecla del teclado
        int idx = FindKeyboardPartIndex(actionRef.action, compositePartName);

        string display = idx < 0
            ? "-"
            : actionRef.action.GetBindingDisplayString(idx, out _, out _, InputBinding.DisplayStringOptions.DontOmitDevice);

        if (display != lastShown)
        {
            lastShown = display;
            bindingText.text = display;
        }

        // ✅ Solo permitimos cambiar si estás usando teclado
        if (rebindButton != null)
            rebindButton.interactable = !usingGamepad;
    }

    // 🔥 Función robusta: encuentra Up/Down/Left/Right del teclado sin usar groups ni "WASD"
    private int FindKeyboardPartIndex(InputAction action, string partName)
    {
        if (action == null) return -1;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];

            if (!b.isPartOfComposite) continue;

            // Up/Down/Left/Right a veces vienen en minúsculas: "up"
            if (!string.Equals(b.name, partName, System.StringComparison.OrdinalIgnoreCase))
                continue;

            // Nos aseguramos de que sea teclado
            string path = b.effectivePath;
            if (string.IsNullOrEmpty(path)) path = b.path;

            if (!string.IsNullOrEmpty(path) && path.Contains("<Keyboard>"))
                return i;
        }

        return -1;
    }

    public void StartRebindKeyboard()
    {
        if (actionRef == null || actionRef.action == null)
            return;

        if (deviceDetector != null && deviceDetector.IsUsingGamepad())
            return;

        var action = actionRef.action;

        int bindingIndex = FindKeyboardPartIndex(action, compositePartName);
        if (bindingIndex < 0)
            return;

        DisposeOp();

        if (rebindButton != null) rebindButton.interactable = false;
        if (rebindButtonText != null) rebindButtonText.text = "Pulsa...";
        if (bindingText != null) bindingText.text = "Esperando...";

        action.Disable();

        op = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough(cancelPath)
            .OnMatchWaitForAnother(matchWaitSeconds)
            .WithControlsExcluding("<Gamepad>"); // ✅ teclado only

        op.OnComplete(_ =>
        {
            action.Enable();
            Finish();
            saveLoad?.Save();
            Refresh();
        });

        op.OnCancel(_ =>
        {
            action.Enable();
            Finish();
            Refresh();
        });

        op.Start();
    }

    private void Finish()
    {
        DisposeOp();
        if (rebindButton != null) rebindButton.interactable = true;
        if (rebindButtonText != null) rebindButtonText.text = "Cambiar";
    }

    private void DisposeOp()
    {
        op?.Dispose();
        op = null;
    }
}