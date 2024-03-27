using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

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
    private PlayerController.State[] canShootStates;
    private Rigidbody2D nearestEnemy;
    [SerializeField]
    private LayerMask enemiesLayer;
    [SerializeField]
    private LayerMask mapLayer;
    [SerializeField]
    private ParticleSystem shootParticles;
    private Animator shootinAnim;

    [Space, Header("Audio"), SerializeField]
    private AudioClip shootClip;

    [Space, Header("AutoShoot"), SerializeField]
    private bool autoShoot;
    [SerializeField]
    private GameObject aimTarget;

    [Space, SerializeField]
    private bool showGizmos;

    private PlayerController playerController;

    Slider sliderHealthBar;


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        shootinAnim = GetComponentInChildren<Animator>();
        sliderHealthBar = aimTarget.GetComponentInChildren<Slider>();
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, shootingRange, Vector2.zero, 0, enemiesLayer);

        float minDisntance = 100;
        Rigidbody2D foundEnemy = null;
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.rigidbody)
                continue;

            float distance = Vector2.Distance(transform.position, hit.point);
            float multuplyValue = (hit.rigidbody.transform.position - transform.position).magnitude;
            Vector3 dir = (hit.rigidbody.transform.position - transform.position);
            if (minDisntance > distance && 
                !Physics2D.Raycast(transform.position, dir.normalized, multuplyValue, mapLayer))
            {
                Debug.DrawLine(transform.position, transform.position + dir.normalized * multuplyValue, Color.green, 0.01f);
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
            Enemy currentEnemy = nearestEnemy.GetComponent<Enemy>();
            if (currentEnemy)
                sliderHealthBar.value = currentEnemy.currentHealth / currentEnemy.maxHealth;
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
        if (playerController.GetState() == PlayerController.State.MINING)
            return;

        currentDelay += Time.fixedDeltaTime * TimeManager.Instance.timeParameter;
        if (autoShoot)
        {
            isShooting = nearestEnemy;
        }

        if (currentDelay >= reloadDelay && isShooting && CheckPlayerState())
        {
            AudioManager.instance.Play2dOneShotSound(shootClip, "Laser");

            currentDelay = 0;
            Instantiate(laserPrefab, posToSpawnBullets.position, transform.rotation);
            CameraController.Instance.AddLowTrauma();
            shootinAnim.SetTrigger("Shoot");
            shootParticles.Play();
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
