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

    [SerializeField]
    InputActionReference shootAction;

    PlayerController playerController;

    CannonController cannonController;

    public Vector2 inputMovementDirection { get; private set; }
    public Vector2 inputAimTurretDirection { get; private set; }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        cannonController = GetComponentInChildren<CannonController>();
    }
    private void OnEnable()
    {
        moveAction.action.started += MoveAction;
        moveAction.action.performed += MoveAction;
        moveAction.action.canceled += MoveAction;

        aimTurretAction.action.started += AimTurretAction;
        aimTurretAction.action.performed += AimTurretAction;
        aimTurretAction.action.canceled += AimTurretAction;

        shootAction.action.started += ShootAction;
        shootAction.action.canceled += ShootAction;
    }

    private void OnDisable()
    {
        moveAction.action.started -= MoveAction;
        moveAction.action.performed -= MoveAction;
        moveAction.action.canceled -= MoveAction;

        aimTurretAction.action.started -= AimTurretAction;
        aimTurretAction.action.performed -= AimTurretAction;
        aimTurretAction.action.canceled -= AimTurretAction;

        shootAction.action.started -= ShootAction;
        shootAction.action.canceled -= ShootAction;
    }

    private void MoveAction(InputAction.CallbackContext obj)
    {
        inputMovementDirection = moveAction.action.ReadValue<Vector2>();
    }

    private void AimTurretAction(InputAction.CallbackContext obj)
    {
        inputAimTurretDirection = aimTurretAction.action.ReadValue<Vector2>();
    }

    private void ShootAction(InputAction.CallbackContext obj)
    {
        if(obj.started)
        {
            cannonController.canShoot = true;
        }
        else
        {
            cannonController.canShoot = false;
        }
    }

    
}
