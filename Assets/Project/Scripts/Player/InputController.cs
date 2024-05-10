using UnityEngine;
using UnityEngine.InputSystem;


public class InputController : MonoBehaviour
{
    public static InputController Instance;

    public enum ControllerType { KEYBOARD, GAMEPAD }

    private PlayerInput inputSystem;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputSystem = FindObjectOfType<PlayerInput>();
    }

    public void ChangeActionMap(string _nextActionMap)
    {
        if (inputSystem)
            inputSystem.SwitchCurrentActionMap(_nextActionMap);
    }

    public ControllerType GetControllerType()
    {
        if (Gamepad.current == null)
            return ControllerType.KEYBOARD;
        else
            return ControllerType.GAMEPAD;

    }

}



