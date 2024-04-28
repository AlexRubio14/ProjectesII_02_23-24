using UnityEngine;
using UnityEngine.UI; 

public class CannonController : MonoBehaviour
{
    [Space, Header("External References"), SerializeField]
    private Transform posToSpawnBullets;
    [SerializeField]
    private GameObject laserPrefab;

    [Space, Header("Shoot"), SerializeField]
    private float shortShootingRange;
    [SerializeField]
    private float longShootingRange;
    private float currentShootingRange;
    [SerializeField]
    private float reloadDelay;
    private float currentDelay;
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
    [Space, SerializeField]
    private GameObject aimTarget;

    [Space, Header("Audio"), SerializeField]
    private AudioClip shootClip;

    

    [Space, SerializeField]
    private bool showGizmos;

    private PlayerController playerController;

    private Slider sliderHealthBar;


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        shootinAnim = GetComponentInChildren<Animator>();
        sliderHealthBar = aimTarget.GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        BossManager.Instance.onBossEnter += SetLongAttackShootRange;
        BossManager.Instance.onBossExit += SetShortAttackShootRange;

        SetShortAttackShootRange();
    }

    private void OnDisable()
    {
        BossManager.Instance.onBossEnter += SetLongAttackShootRange;
        BossManager.Instance.onBossExit += SetShortAttackShootRange;
    }

    private void FixedUpdate()
    {
        CheckNearestEnemy();
        SetAimTarget();
        RotateCanon();
        Shoot();
    }

    private void CheckNearestEnemy()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, currentShootingRange, Vector2.zero, 0, enemiesLayer);

        nearestEnemy = FindNearestHit(hits);

        
    }

    private Rigidbody2D FindNearestHit(RaycastHit2D[] _hits)
    {
        float minDisntance = 100;
        Rigidbody2D foundEnemy = null;
        foreach (RaycastHit2D hit in _hits)
        {
            if (!hit.rigidbody)
                continue;

            float distance = Vector2.Distance(transform.position, hit.point);
            float multuplyValue = (hit.collider.transform.position - transform.position).magnitude;
            Vector3 dir = (hit.collider.transform.position - transform.position).normalized;

            if (minDisntance > distance &&
                !Physics2D.Raycast(transform.position, dir, multuplyValue, mapLayer))
            {
                Debug.DrawLine(transform.position, transform.position + dir * multuplyValue, Color.green, 0.01f);
                foundEnemy = hit.rigidbody;
                minDisntance = distance;
            }
        }

        return foundEnemy;
    }

    private void SetAimTarget()
    {
        if (!nearestEnemy)
        {
            if (aimTarget.activeInHierarchy)
                aimTarget.SetActive(false);
            return;
        }

        Enemy currentEnemy = nearestEnemy.GetComponent<Enemy>();
        if (!currentEnemy) return;

        if (!aimTarget.activeInHierarchy)
        {
            aimTarget.SetActive(true);
        }

        aimTarget.transform.position = nearestEnemy.transform.position;

        sliderHealthBar.value = currentEnemy.currentHealth / currentEnemy.maxHealth;
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

        if (currentDelay >= reloadDelay && nearestEnemy && CheckPlayerState())
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

    public void SetLongAttackShootRange()
    {
        currentShootingRange = longShootingRange;
    }

    public void SetShortAttackShootRange()
    {
        currentShootingRange = shortShootingRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentShootingRange);
    }
}
