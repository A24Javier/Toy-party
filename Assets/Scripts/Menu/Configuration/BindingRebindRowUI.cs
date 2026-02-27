using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindingRebindRowUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DeviceDetector deviceDetector;
    [SerializeField] private InputActionReference actionRef;

    [Header("UI")]
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private TMP_Text rebindButtonText; // opcional (puede ser null)

    [Header("Scheme names (exactos)")]
    [SerializeField] private string keyboardSchemeName = "Keyboard&Mouse";
    [SerializeField] private string gamepadSchemeName = "Gamepad";

    [Header("Rebind")]
    [SerializeField] private float matchWaitSeconds = 0.1f;
    [SerializeField] private string cancelPath = "<Keyboard>/escape";
    [SerializeField] private RebindSaveLoad saveLoad;

    private InputActionRebindingExtensions.RebindingOperation op;
    private string lastShown = "";

    private void OnEnable()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.RemoveAllListeners();
            rebindButton.onClick.AddListener(StartRebind);
        }

        RefreshBindingText();
    }

    private void OnDisable()
    {
        DisposeOp();
    }

    private void Update()
    {
        // refresco ligero por si cambia dispositivo
        RefreshBindingText();
    }

    private void RefreshBindingText()
    {
        if (deviceDetector == null || actionRef == null || actionRef.action == null || bindingText == null) return;

        string scheme = deviceDetector.IsUsingGamepad() ? gamepadSchemeName : keyboardSchemeName;
        string display = GetBindingDisplayForScheme(actionRef.action, scheme);

        if (display == lastShown) return;
        lastShown = display;
        bindingText.text = display;
    }

    private int FindBindingIndexForScheme(InputAction action, string scheme)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (b.isComposite || b.isPartOfComposite) continue;

            if (!string.IsNullOrEmpty(b.groups) && b.groups.Contains(scheme))
                return i;
        }
        return -1;
    }

    private string GetBindingDisplayForScheme(InputAction action, string scheme)
    {
        int idx = FindBindingIndexForScheme(action, scheme);
        if (idx < 0) return "-";

        return action.GetBindingDisplayString(
            idx,
            out _,
            out _,
            InputBinding.DisplayStringOptions.DontOmitDevice
        );
    }

    public void StartRebind()
    {
        if (deviceDetector == null || actionRef == null || actionRef.action == null) return;

        var action = actionRef.action;

        string scheme = deviceDetector.IsUsingGamepad() ? gamepadSchemeName : keyboardSchemeName;
        int bindingIndex = FindBindingIndexForScheme(action, scheme);
        if (bindingIndex < 0) return;

        // Evitar doble operación
        DisposeOp();

        // UI feedback
        if (rebindButton != null) rebindButton.interactable = false;
        if (rebindButtonText != null) rebindButtonText.text = "Pulsa...";
        bindingText.text = "Esperando input...";

        action.Disable();

        op = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough(cancelPath)
            .OnMatchWaitForAnother(matchWaitSeconds);

        // Filtrar: si estás en teclado, excluye mando; si estás en mando, excluye teclado/ratón
        if (scheme == keyboardSchemeName)
        {
            op.WithControlsExcluding("<Gamepad>");
        }
        else if (scheme == gamepadSchemeName)
        {
            op.WithControlsExcluding("<Keyboard>");
            op.WithControlsExcluding("<Mouse>");
        }

        op.OnComplete(_ =>
        {
            action.Enable();
            FinishRebindUI();
            SaveOverridesIfYouWant();
            RefreshBindingText();
        });

        op.OnCancel(_ =>
        {
            action.Enable();
            FinishRebindUI();
            RefreshBindingText();
        });

        op.Start();
    }

    private void FinishRebindUI()
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

    // Paso siguiente: lo conectamos a PlayerPrefs para persistir.
    private void SaveOverridesIfYouWant()
    {
        if (saveLoad != null)
            saveLoad.Save();
    }
}