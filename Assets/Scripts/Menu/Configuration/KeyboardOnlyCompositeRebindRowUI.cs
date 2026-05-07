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

    private void OnEnable()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.RemoveAllListeners();
            rebindButton.onClick.AddListener(StartRebind);
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (op != null)
        {
            op.Dispose();
            op = null;
        }

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private void Update()
    {
        Refresh();
    }

    public void StartRebind()
    {
        if (deviceDetector != null && deviceDetector.IsUsingGamepad())
            return;

        if (actionRef == null || actionRef.action == null)
        {
            Debug.LogWarning($"{name}: falta actionRef o actionRef.action.");
            return;
        }

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(RebindRoutine());
    }

    private IEnumerator RebindRoutine()
    {
        InputAction action = actionRef.action;

        int bindingIndex = FindKeyboardPart(action);
        if (bindingIndex < 0)
        {
            Debug.LogWarning($"{name}: no se encontró binding composite '{compositePartName}'.");
            yield break;
        }

        SetUILocked(true);

        if (bindingText != null)
            bindingText.text = "Suelta y pulsa...";

        yield return new WaitUntil(AllInputsReleased);

        action.Disable();

        op = action.PerformInteractiveRebinding(bindingIndex)
            .WithMatchingEventsBeingSuppressed()
            .WithCancelingThrough("<Keyboard>/escape");

        string currentPath = action.bindings[bindingIndex].effectivePath;

        op.OnPotentialMatch(o =>
        {
            if (o.selectedControl != null && o.selectedControl.path == currentPath)
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
        if (action != null)
            action.Enable();

        if (op != null)
        {
            op.Dispose();
            op = null;
        }

        SetUILocked(false);

        if (saveLoad != null)
            saveLoad.Save();

        lastShown = "";
        Refresh();
    }

    private void SetUILocked(bool locked)
    {
        if (otherButtonsToDisable != null)
        {
            foreach (Button b in otherButtonsToDisable)
            {
                if (b != null)
                    b.interactable = !locked;
            }
        }

        if (rebindButton != null)
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
        if (action == null)
            return -1;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding b = action.bindings[i];

            if (!b.isPartOfComposite)
                continue;

            if (!string.Equals(b.name, compositePartName, System.StringComparison.OrdinalIgnoreCase))
                continue;

            string path = b.effectivePath;

            if (!string.IsNullOrEmpty(path) && path.Contains("<Keyboard>"))
                return i;
        }

        return -1;
    }

    private void Refresh()
    {
        if (actionRef == null || actionRef.action == null)
            return;

        if (bindingText == null)
            return;

        int idx = FindKeyboardPart(actionRef.action);
        if (idx < 0)
            return;

        string display = actionRef.action.GetBindingDisplayString(idx);

        if (display == lastShown)
            return;

        lastShown = display;
        bindingText.text = display;

        if (rebindButton != null)
        {
            bool usingGamepad = deviceDetector != null && deviceDetector.IsUsingGamepad();
            rebindButton.interactable = !usingGamepad;
        }
    }
}