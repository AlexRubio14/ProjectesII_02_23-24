using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField]
    InputActionReference moveAction;

    [SerializeField]
    InputActionReference aimTurretAction;

    PlayerController pController;

    public Vector2 inputMovementDirection { get; private set; }
    public Vector2 inputAimTurretDirection { get; private set; }

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        pController = GetComponent<PlayerController>();

        moveAction.action.started += MoveAction;
        moveAction.action.performed += MoveAction;
        moveAction.action.canceled += MoveAction;

        aimTurretAction.action.started += AimTurretAction;
        aimTurretAction.action.performed += AimTurretAction;
        aimTurretAction.action.canceled += AimTurretAction;


    }

    private void OnDisable()
    {
        moveAction.action.started -= MoveAction;
        moveAction.action.performed -= MoveAction;
        moveAction.action.canceled -= MoveAction;

        aimTurretAction.action.started += AimTurretAction;
        aimTurretAction.action.performed += AimTurretAction;
        aimTurretAction.action.canceled += AimTurretAction;
    }

    private void MoveAction(InputAction.CallbackContext obj)
    {
        inputMovementDirection = moveAction.action.ReadValue<Vector2>();
    }

    private void AimTurretAction(InputAction.CallbackContext obj)
    {
        inputAimTurretDirection = aimTurretAction.action.ReadValue<Vector2>();
    }
}
