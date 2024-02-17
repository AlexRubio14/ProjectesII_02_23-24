using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirtualMouseUI : MonoBehaviour
{

    private VirtualMouseInput virtualMouseInput;

    [SerializeField, Range(0f, 1f)]
    private float screenCut;
    [SerializeField]
    private RectTransform pointer;

    void Awake()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();        
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnInputDeviceChanged; 
    }
    private void OnDisable()
    {
        InputSystem.onActionChange -= OnInputDeviceChanged;
    }
    void LateUpdate()
    {
        MouseUpdate();
        
    }

    private void OnInputDeviceChanged(object arg1, InputActionChange arg2)
    {
        //virtualMouseInput.enabled = InputController.Instance.GetCurrentControllerType() == InputController.ControllerType.GAMEPAD;
    }

    private void MouseUpdate()
    {
        Vector2 cursorPosition = Vector2.zero;

        switch (InputController.Instance.GetCurrentControllerType())
        {
            case InputController.ControllerType.KEYBOARD:

                cursorPosition = ClampMousePosition(Mouse.current.position.value);

                break;
            case InputController.ControllerType.GAMEPAD:
                cursorPosition = ClampMousePosition(virtualMouseInput.virtualMouse.position.value);
                break;
            default:
                break;
        }

        SetPointerMousePosition(cursorPosition);
    }

    private Vector2 ClampMousePosition(Vector2 _newPosition)
    {
        Vector2 newVMousePos = _newPosition;
        newVMousePos.x = Mathf.Clamp(newVMousePos.x, 0, Screen.width);
        newVMousePos.y = Mathf.Clamp(newVMousePos.y, Screen.height * screenCut, Screen.height);
        InputState.Change(virtualMouseInput.virtualMouse.position, newVMousePos);

        return newVMousePos;
    }

    private void SetPointerMousePosition(Vector2 _cursorPos)
    {
        pointer.anchoredPosition = Camera.main.ScreenToViewportPoint(_cursorPos) * new Vector2(Screen.width, Screen.height);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height * screenCut, 200));
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * screenCut, 200));
        Gizmos.DrawLine(startPos, endPos);
    }

}
