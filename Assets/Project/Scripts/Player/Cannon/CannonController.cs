using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    [Header("Input"), SerializeField]
    private InputActionReference shootAction;

    [Space, Header("External References"), SerializeField]
    private Transform posToSpawnBullets;
    [SerializeField]
    private GameObject laserPrefab;

    [Space, Header("Shoot"), SerializeField]
    private float shootingRange;
    [SerializeField]
    private float reloadDelay;
    private float currentDelay;
    private bool isShooting;
    [SerializeField]
    private float fuelConsume;
    [SerializeField]
    private PlayerController.State[] canShootStates;
    private Rigidbody2D nearestEnemy;
    [SerializeField]
    private LayerMask enemiesMask;

    [Space, SerializeField]
    private GameObject aimTarget;

    [Space, SerializeField]
    private bool autoShoot;

    private PlayerController playerController;

    [SerializeField]
    private AudioClip shootClip;

    [Space, SerializeField]
    private bool showGizmos;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        isShooting = false;
    }

    private void OnEnable()
    {
        shootAction.action.started += ShootAction;
        shootAction.action.canceled += ShootAction;
    }

    private void OnDisable()
    {
        shootAction.action.started -= ShootAction;
        shootAction.action.canceled -= ShootAction;
    }

    private void FixedUpdate()
    {
        CheckNearestEnemy();
        RotateCanon();
        Shoot();
    }

    private void CheckNearestEnemy()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, shootingRange, Vector2.zero, 0, enemiesMask);

        float minDisntance = 100;
        Rigidbody2D foundEnemy = null;
        int index = 0;
        foreach (RaycastHit2D hit in hits)
        {
            index++;
            float distance = Vector2.Distance(transform.position, hit.point);
            if (minDisntance > distance)
            {
                foundEnemy = hit.rigidbody;
                minDisntance = distance;
            }
        }

        nearestEnemy = foundEnemy;

        if (nearestEnemy)
        {
            if (!aimTarget.activeInHierarchy)
            {
                aimTarget.SetActive(true);
            }

            aimTarget.transform.position = nearestEnemy.transform.position;
        }
        else if (aimTarget.activeInHierarchy)
            aimTarget.SetActive(false);
    }
    private void RotateCanon()
    {
        if (!nearestEnemy)
            return;

        Vector2 direction = (nearestEnemy.transform.position - transform.position).normalized;
        transform.up = direction;
    }
    public void Shoot()
    {
        currentDelay += Time.fixedDeltaTime;
        if (autoShoot)
        {
            isShooting = nearestEnemy;
        }
        

        if (currentDelay >= reloadDelay && isShooting && CheckPlayerState())
        {
            AudioManager._instance.Play2dOneShotSound(shootClip, "Laser");

            currentDelay = 0;
            Instantiate(laserPrefab, posToSpawnBullets.position, transform.rotation);
            CameraController.Instance.AddLowTrauma();
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
    private void ShootAction(InputAction.CallbackContext obj)
    {
        isShooting = obj.action.IsInProgress();
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
