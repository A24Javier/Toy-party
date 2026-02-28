using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardMoveCompositeRowUI : MonoBehaviour
{
    [SerializeField] private DeviceDetector deviceDetector;
    [SerializeField] private RebindSaveLoad saveLoad;
    [SerializeField] private InputActionReference actionRef;

    [SerializeField] private string compositePartName = "Up";

    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private Button[] otherButtonsToDisable;

    private InputActionRebindingExtensions.RebindingOperation op;
    private Coroutine routine;

    private EventSystem cachedEventSystem;
    private bool prevSendNavEvents;
    private bool prevEventSystemEnabled;

    private string lastShown = "";

    void OnEnable()
    {
        rebindButton.onClick.RemoveAllListeners();
        rebindButton.onClick.AddListener(StartRebind);
        Refresh();
    }

    void Update() => Refresh();

    public void StartRebind()
    {
        if (deviceDetector.IsUsingGamepad()) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RebindRoutine());
    }

    private IEnumerator RebindRoutine()
    {
        var action = actionRef.action;
        int bindingIndex = FindKeyboardPart(action);
        if (bindingIndex < 0) yield break;

        SetUILocked(true);

        bindingText.text = "Suelta y pulsa...";
        yield return new WaitUntil(AllInputsReleased);

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

        lastShown = "";
        Refresh();
    }

    private void SetUILocked(bool locked)
    {
        if (otherButtonsToDisable != null)
        {
            foreach (var b in otherButtonsToDisable)
                if (b != null) b.interactable = !locked;
        }

        rebindButton.interactable = !locked;

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

    private int FindKeyboardPart(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];

            if (!b.isPartOfComposite) continue;

            if (!string.Equals(b.name, compositePartName,
                System.StringComparison.OrdinalIgnoreCase))
                continue;

            string path = b.effectivePath;
            if (!string.IsNullOrEmpty(path) && path.Contains("<Keyboard>"))
                return i;
        }
        return -1;
    }

    private void Refresh()
    {
        int idx = FindKeyboardPart(actionRef.action);
        if (idx < 0) return;

        string display = actionRef.action.GetBindingDisplayString(idx);

        if (display == lastShown) return;

        lastShown = display;
        bindingText.text = display;

        rebindButton.interactable = !deviceDetector.IsUsingGamepad();
    }
}