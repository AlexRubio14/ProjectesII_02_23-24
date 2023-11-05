using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference aimTurretAction;
    [SerializeField]
    private InputActionReference shootAction;
    [SerializeField]
    private InputActionReference interactAction;

    private PlayerController playerController;
    private CannonController cannonController;
    private MapInteractPlayer c_interactionController;

    public Vector2 inputMovementDirection { get; private set; }
    public Vector2 inputAimTurretDirection { get; private set; }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        c_interactionController = GetComponent<MapInteractPlayer>();
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

        interactAction.action.started += InteractAction;
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

        interactAction.action.started -= InteractAction;
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

    private void InteractAction(InputAction.CallbackContext obj)
    {
        c_interactionController.InteractNearObject();
    }
}
