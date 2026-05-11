using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindingRebindRowUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DeviceDetector deviceDetector;
    [SerializeField] private RebindSaveLoad saveLoad;
    [SerializeField] private InputActionReference actionRef;

    [Header("UI")]
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private TMP_Text rebindButtonText;

    [Header("Schemes")]
    [SerializeField] private string keyboardScheme = "Keyboard&Mouse";
    [SerializeField] private string gamepadScheme = "Gamepad";

    [Header("UI Lock")]
    [SerializeField] private Button[] otherButtonsToDisable;
    [SerializeField] private bool disableUINavigationWhileRebinding = true;

    private InputActionRebindingExtensions.RebindingOperation op;
    private Coroutine routine;

    private EventSystem cachedEventSystem;
    private bool prevSendNavEvents;
    private bool prevEventSystemEnabled;

    private string lastShown = "";

    void OnEnable()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.RemoveAllListeners();
            rebindButton.onClick.AddListener(StartRebind);
        }

        RefreshBindingText();
    }

    void Update() => RefreshBindingText();

    // =========================
    public void StartRebind()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RebindRoutine());
    }

    private IEnumerator RebindRoutine()
    {
        var action = actionRef.action;

        int bindingIndex = FindBindingIndex(action);
        if (bindingIndex < 0) yield break;

        SetUILocked(true);

        bindingText.text = "Suelta y pulsa...";
        yield return new WaitUntil(AllInputsReleased);

        bindingText.text = "Esperando input...";

        action.Disable();

        op = action.PerformInteractiveRebinding(bindingIndex)
            .WithMatchingEventsBeingSuppressed()
            .WithCancelingThrough("<Keyboard>/escape");

        string currentPath = action.bindings[bindingIndex].effectivePath;

        op.OnPotentialMatch(o =>
        {
            if (o.selectedControl != null &&
                o.selectedControl.path == currentPath)
                o.Complete();
        });

        op.OnComplete(o =>
        {
            if (o.selectedControl != null)
                action.ApplyBindingOverride(bindingIndex, o.selectedControl.path);

            Finish(action);
        });

        op.OnCancel(o => Finish(action));

        op.Start();
    }

    private void Finish(InputAction action)
    {
        action.Enable();

        op?.Dispose();
        op = null;

        SetUILocked(false);
        saveLoad?.Save();

        if (rebindButtonText != null)
            rebindButtonText.text = "Cambiar";

        lastShown = "";
        RefreshBindingText();
    }

    // =========================
    private void SetUILocked(bool locked)
    {
        if (otherButtonsToDisable != null)
        {
            foreach (var b in otherButtonsToDisable)
                if (b != null) b.interactable = !locked;
        }

        if (rebindButton != null)
            rebindButton.interactable = !locked;

        if (!disableUINavigationWhileRebinding)
            return;

        if (locked)
        {
            if (cachedEventSystem == null)
                cachedEventSystem = EventSystem.current;

            if (cachedEventSystem != null)
            {
                prevSendNavEvents = cachedEventSystem.sendNavigationEvents;
                prevEventSystemEnabled = cachedEventSystem.enabled;

                cachedEventSystem.sendNavigationEvents = false;
                cachedEventSystem.SetSelectedGameObject(null);
                cachedEventSystem.enabled = false;
            }
        }
        else
        {
            if (cachedEventSystem != null)
            {
                cachedEventSystem.enabled = prevEventSystemEnabled;
                cachedEventSystem.sendNavigationEvents = prevSendNavEvents;
            }
        }
    }

    private bool AllInputsReleased()
    {
        return Keyboard.current == null || !Keyboard.current.anyKey.isPressed;
    }

    private int FindBindingIndex(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (!b.isComposite && !b.isPartOfComposite)
                return i;
        }
        return -1;
    }

    private void RefreshBindingText()
    {
        if (actionRef == null || actionRef.action == null || bindingText == null)
            return;

        var action = actionRef.action;
        int idx = FindBindingIndex(action);
        if (idx < 0) return;

        string display = action.GetBindingDisplayString(idx);

        if (display == lastShown) return;

        lastShown = display;
        bindingText.text = display;
    }
}