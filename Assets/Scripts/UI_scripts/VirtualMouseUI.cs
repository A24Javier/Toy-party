using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class VirtualMouseUI : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRectTransform;
    private VirtualMouseInput _virtualMouse;

    [SerializeField] private Image _virtualMouseVisual;

    private InputAction _stickAction;
    private InputAction _leftButtonAction;

    private float _initialCursorSpeed = 1000f;
    private bool _isEnabled = false;

    void Awake()
    {
        _virtualMouse = GetComponent<VirtualMouseInput>();
        _stickAction = _virtualMouse.stickAction.action;
        _leftButtonAction = _virtualMouse.leftButtonAction.action;
    }

    void Start()
    {
        _initialCursorSpeed = _virtualMouse.cursorSpeed;

        if(Gamepad.current != null)
        {
            EnableVirtualMouse();
        }
        else
        {
            DisableVirtualMouse();
        }
    }

    private void Update()
    {
#if RELEASE_BUILD
        if (Input.anyKeyDown && _isEnabled)
        {
            DisableVirtualMouse();
        }
        else if (Gamepad.current != null && !_isEnabled)
        {
            if (_stickAction.IsPressed() || _leftButtonAction.IsPressed())
            {
                EnableVirtualMouse();
            }
        }

#elif UNITY_EDITOR
        if (Input.anyKeyDown && _isEnabled && !_stickAction.IsPressed() && !_leftButtonAction.IsPressed())
        {
            DisableVirtualMouse();
        }
        else if (!_isEnabled)
        {
            if (_stickAction.IsPressed() || _leftButtonAction.IsPressed())
            {
                EnableVirtualMouse();
            }
        }
#endif
        transform.localScale = Vector3.one * (1f / _canvasRectTransform.localScale.x);
        transform.SetAsLastSibling();
    }

    void LateUpdate()
    {
        if (!_isEnabled)
            return;

        Vector2 virtualMousePos = _virtualMouse.virtualMouse.position.value;
        virtualMousePos.x = Mathf.Clamp(virtualMousePos.x, 0f, Screen.width);
        virtualMousePos.y = Mathf.Clamp(virtualMousePos.y, 0f, Screen.height);

        InputState.Change(_virtualMouse.virtualMouse.position, virtualMousePos);
    }

    private void DisableVirtualMouse()
    {
        _isEnabled = false;

        _virtualMouse.cursorSpeed = 0f;
        _virtualMouseVisual.enabled = false;
    }
    private void EnableVirtualMouse()
    {
        _isEnabled = true;

        _virtualMouse.cursorSpeed = _initialCursorSpeed;
        _virtualMouseVisual.enabled = true;
    }

}
