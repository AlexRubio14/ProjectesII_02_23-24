using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputController;

public class InputController : MonoBehaviour
{
    public enum ControllerType { KEYBOARD, GAMEPAD }

    private PlayerInput c_playerInput;

    private void Awake()
    {
        c_playerInput = GetComponent<PlayerInput>();
    }

    public void ChangeActionMap(string _nextActionMap)
    {
        c_playerInput.SwitchCurrentActionMap(_nextActionMap);
    }

    public ControllerType GetCurrentControllerType()
    {
        ControllerType currentType;
        if (Gamepad.current == null)
        {
            Debug.Log("Mouse");
            currentType = ControllerType.KEYBOARD;
        }
        else
        {
            Debug.Log("GamePad");
            currentType = ControllerType.GAMEPAD;
        }

        return currentType;
    }

}



