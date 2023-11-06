using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("Ingame Actions"), SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference aimTurretAction;
    [SerializeField]
    private InputActionReference shootAction;
    [SerializeField]
    private InputActionReference interactAction;
    [SerializeField]
    private InputActionReference inventoryAction;
    
    [Space, Header("Minigame Actions"), SerializeField]
    private InputActionReference chargeRightLaserAction;
    [SerializeField]
    private InputActionReference chargeLeftLaserAction;

    private PlayerInput c_playerInput;
    private PlayerController c_playerController;
    private CannonController c_cannonController;
    private MapInteractPlayer c_interactionController;
    private PlayerMineryController c_mineryController;
    private PlayerInventoryController c_inventoryController;

    public Vector2 inputMovementDirection { get; private set; }
    public Vector2 inputAimTurretDirection { get; private set; }

    private void Awake()
    {
        c_playerInput = GetComponent<PlayerInput>();
        c_playerController = GetComponent<PlayerController>();
        c_interactionController = GetComponent<MapInteractPlayer>();
        c_cannonController = GetComponentInChildren<CannonController>();
        c_mineryController = GetComponent<PlayerMineryController>();
        c_inventoryController = GetComponent<PlayerInventoryController>();

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

        inventoryAction.action.started += InventoryAction;


        chargeRightLaserAction.action.started += ChargeRightLaserAction;
        chargeRightLaserAction.action.canceled += ChargeRightLaserAction;

        chargeLeftLaserAction.action.started += ChargeLeftLaserAction;
        chargeLeftLaserAction.action.canceled += ChargeLeftLaserAction;

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

        inventoryAction.action.started -= InventoryAction;



        chargeRightLaserAction.action.started -= ChargeRightLaserAction;
        chargeRightLaserAction.action.canceled -= ChargeRightLaserAction;

        chargeLeftLaserAction.action.started -= ChargeLeftLaserAction;
        chargeLeftLaserAction.action.canceled -= ChargeLeftLaserAction;

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
            c_cannonController.canShoot = true;
        }
        else
        {
            c_cannonController.canShoot = false;
        }
    }

    private void InteractAction(InputAction.CallbackContext obj)
    {
        c_interactionController.InteractNearObject();
    }

    private void InventoryAction(InputAction.CallbackContext obj)
    {
        c_inventoryController.ChangeInventoryVisibility();
    }

    private void ChargeRightLaserAction(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            c_mineryController.SetRightLaserValue(false);
        }
        else
        {
            c_mineryController.SetRightLaserValue(true);
        }
    }
    private void ChargeLeftLaserAction(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            c_mineryController.SetLeftLaserValue(false);
        }
        else
        {
            c_mineryController.SetLeftLaserValue(true);
        }
    }
    public void ChangeActionMap(string _nextActionMap)
    {
        c_playerInput.SwitchCurrentActionMap(_nextActionMap);
    }

}
