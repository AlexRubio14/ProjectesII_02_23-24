using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    [Header("Input"), SerializeField]
    private InputActionReference aimTurretAction;
    [SerializeField]
    private InputActionReference shootAction;

    [Space, Header("External References"), SerializeField]
    private Transform posToSpawnBullets;
    [SerializeField]
    private GameObject laserPrefab;

    [Space, Header("Shoot"), SerializeField]
    private float reloadDelay;
    private float currentDelay;
    private bool isShooting;
    [SerializeField]
    private float fuelConsume;
    private Vector2 turretAimDirection;
    [SerializeField]
    private PlayerController.State[] canShootStates; 

    private InputController iController;
    private PlayerController playerController;

    private void Awake()
    {
        iController = GetComponentInParent<InputController>();
        playerController = GetComponentInParent<PlayerController>();
        isShooting = false;
    }

    private void OnEnable()
    {
        aimTurretAction.action.started += AimTurretAction;
        aimTurretAction.action.performed += AimTurretAction;
        aimTurretAction.action.canceled += AimTurretAction;

        shootAction.action.started += ShootAction;
        shootAction.action.canceled += ShootAction;
    }

    private void OnDisable()
    {
        aimTurretAction.action.started -= AimTurretAction;
        aimTurretAction.action.performed -= AimTurretAction;
        aimTurretAction.action.canceled -= AimTurretAction;

        shootAction.action.started -= ShootAction;
        shootAction.action.canceled -= ShootAction;
    }

    private void FixedUpdate()
    {
        RotateCanon();
        Shoot();
    }

    private void RotateCanon()
    {
        if(iController.GetCurrentControllerType() == InputController.ControllerType.KEYBOARD) 
        {
            Vector2 direction = (GetMousePosition() - (Vector2)transform.position).normalized;

            transform.up = direction;
        }
        else
        {
            if (turretAimDirection.normalized == Vector2.zero)
            {
                return;
            }
            transform.up = turretAimDirection.normalized;
        }
    }
    public void Shoot()
    {
        currentDelay += Time.fixedDeltaTime;

        if (currentDelay >= reloadDelay && isShooting && CheckPlayerState())
        {
            currentDelay = 0;
            Instantiate(laserPrefab, posToSpawnBullets.position, transform.rotation);
            CameraController.Instance.SetTrauma(0.4f);
            playerController.SubstractHealth(fuelConsume);
        }
    }
    private bool CheckPlayerState()
    {
        PlayerController.State currentState = playerController.GetState();

        foreach (PlayerController.State item in canShootStates)
        {
            if (item == currentState)
            {
                return true;
            }
        }

        return false;

    }



    #region Input
    private void AimTurretAction(InputAction.CallbackContext obj)
    {
        turretAimDirection = aimTurretAction.action.ReadValue<Vector2>();
    }
    private void ShootAction(InputAction.CallbackContext obj)
    {
        isShooting = obj.action.IsInProgress();
    }

    Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return mousePosition;
    }

    #endregion
}
