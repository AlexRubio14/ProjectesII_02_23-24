using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("Inputs"), SerializeField]
    private InputActionReference rotationAction;
    [SerializeField]
    private InputActionReference accelerateAction;
    [SerializeField]
    private InputActionReference dashAction;

    public enum State { IDLE, MOVING, DASHING, MINING, KNOCKBACK, INVENCIBILITY, FREEZE, DEAD};
    private State currentState;

    private Rigidbody2D c_rb;
    [Space, Header("Movement"), SerializeField]
    private float movementSpeed;
    private float accelerationValue;
    private Vector2 movementDirection;
    [HideInInspector]
    public float externalMovementSpeed;

    [Space, Header("Dash"), SerializeField]
    private float dashForce;
    private bool canDash;
    private float lastTimeDash;
    [SerializeField]
    private float timeCanDash;
    [SerializeField]
    private float dashFuelConsume;
    [Space, SerializeField]
    private float timeDashing;
    private float currentDashTime;
    private Vector2 dashDirection;
    [Space, SerializeField]
    private float dashSpeedDrag;

    [Space, Header("Rotation"), SerializeField]
    private float rotationSpeed;

    [Space, Header("Knockback"), SerializeField]
    private float knockbackScale;
    [SerializeField]
    private float knockbackRotation;
    [SerializeField]
    private float knockbackTime;

    [Space, Header("Fuel"), SerializeField]
    private float baseFuel;
    public float fuel { get; private set; }
    [HideInInspector]
    public float fuelConsume;
    [SerializeField]
    private float mapDamage;
    [SerializeField]
    private float idleFuelConsume;
    [SerializeField]
    private float movingFuelConsume;
    [SerializeField]
    private ParticleSystem hitParticles;
    [HideInInspector]
    public Action OnHit;
    public ParticleSystem refillFuelParticles;

    [Space, Header("Death"), SerializeField]
    private GameObject explosionParticles;
    [SerializeField]
    private float timeToExploteShip;
    [SerializeField]
    private float timeToReturnHub;
    private Light2D shipLight;
    [SerializeField]
    private ParticleSystem engineParticles;
    [SerializeField]
    private AudioClip enginePoweringOff;
    [SerializeField]
    private AudioClip deathExplosion;

    [Space, Header("Audio"), SerializeField]
    private AudioClip collisionClip;
    [SerializeField]
    private AudioClip engineClip;
    private AudioSource engineSource;

    private SpriteRenderer spriteRenderer;
    private PlayerMapInteraction mapInteraction;
    private AutoHelpController autoHelpController;
    private SizeUpgradeController sizeUpgrade;

    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();

        mapInteraction = GetComponent<PlayerMapInteraction>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shipLight = GetComponentInChildren<Light2D>();
        autoHelpController = GetComponent<AutoHelpController>();
        sizeUpgrade = GetComponent<SizeUpgradeController>();    

    }
    
    private void Start()
    {
        fuelConsume = -idleFuelConsume;
        currentState = State.IDLE;

        fuel = baseFuel + PowerUpManager.Instance.Fuel;

        canDash = true;

        currentDashTime = 0;
    }

    private void OnEnable()
    {
        rotationAction.action.started += RotateAction;
        rotationAction.action.performed += RotateAction;
        rotationAction.action.canceled += RotateAction;

        accelerateAction.action.started += AccelerateAction;
        accelerateAction.action.performed += AccelerateAction;
        accelerateAction.action.canceled += AccelerateAction;

        dashAction.action.performed += DashAction;

        TimeManager.Instance.pauseAction += PlayerPause;

    }

    private void OnDisable()
    {
        rotationAction.action.started -= RotateAction;
        rotationAction.action.performed -= RotateAction;
        rotationAction.action.canceled -= RotateAction;

        accelerateAction.action.started -= AccelerateAction;
        accelerateAction.action.performed -= AccelerateAction;
        accelerateAction.action.canceled -= AccelerateAction;

        dashAction.action.performed -= DashAction;

        TimeManager.Instance.pauseAction -= PlayerPause;
    }

    void Update()
    {        
        fuel = Mathf.Clamp(fuel, 0, Mathf.Infinity);

        if(Input.GetKey(KeyCode.L))
        {
            fuel += 50;
        }

        CheckIfCanDash();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.IDLE:
            case State.MOVING:
                Move();
                Rotation();
                CheckIdleOrMovingState();
                LoseFuel();
                CheckEnableEngineParticles();
                break;
            case State.DASHING:
                DashingBehaviour();
                break;
            case State.MINING:
                break;
            case State.KNOCKBACK:
                LoseFuel();
                break;
            default:
                break;
        }
    }

    #region Movement
    private void Move()
    {
        Vector2 acceleration =
                        transform.right * //Direccion
                        Mathf.Clamp01(accelerationValue + Mathf.Clamp(externalMovementSpeed, 0, Mathf.Infinity)) * //Aceleracion y en caso de que una fuerza externa sea positiva tambien se sumara
                        Mathf.Clamp(movementSpeed + externalMovementSpeed, 0, Mathf.Infinity); //Velocidad de movimiento sumandole la externa

        c_rb.AddForce(acceleration * TimeManager.Instance.timeParameter * sizeUpgrade.sizeMultiplyer, ForceMode2D.Force);
    }
    private void Rotation()
    {
        Vector2 autoHelp = autoHelpController.autoHelpDirection;
            float autoHelpMutliplier = 3;
            //En caso de que se tenga que aplicar la auto ayuda
            //Y el player no este tocando ningun input
            //Y la ultima direccion no este mirando directo a la pared
            //Se usara la ultima direccion de movimiento
        Vector2 movementAndAutoHelpDirection = movementDirection.normalized * autoHelpMutliplier + autoHelp;
        Vector2 normalizedInputDirection = movementAndAutoHelpDirection.normalized;
        float signedAngle = Vector2.SignedAngle(transform.right, normalizedInputDirection);
        c_rb.AddTorque(signedAngle * (rotationSpeed * sizeUpgrade.sizeMultiplyer) * TimeManager.Instance.timeParameter);
    }
    private void Dash()
    {
        currentState = State.DASHING;

        dashDirection = movementDirection.normalized * dashForce;

        SubstractFuel(dashFuelConsume);
    }

    private void DashingBehaviour()
    {
        currentDashTime += Time.fixedDeltaTime;

        if(currentDashTime >= timeDashing)
        {
            c_rb.velocity = c_rb.velocity / dashSpeedDrag;
            currentDashTime = 0;
            currentState = State.IDLE;
        }
        else
        {
            c_rb.velocity = dashDirection;
        }
    }

    private void CheckIfCanDash()
    {
        lastTimeDash += Time.deltaTime;

        if (lastTimeDash >= timeCanDash) 
        {
            canDash = true;
        }
    }

    private void CheckEnableEngineParticles()
    {
        float moveForce = Mathf.Clamp01(accelerationValue + Mathf.Clamp(externalMovementSpeed, 0, Mathf.Infinity));

        if (engineParticles.isStopped && moveForce > 0 ) // Encender particulas
        {
            engineParticles.Play();
        }
        else if (engineParticles.isPlaying && moveForce == 0) //Apagar particulas
        {
            engineParticles.Stop();
        }

    }

    #endregion

    #region Ship Fuel
    void LoseFuel()
    {
        fuel = Mathf.Clamp(fuel + fuelConsume * Time.fixedDeltaTime * TimeManager.Instance.timeParameter, 0, GetMaxFuel());
        CheckIfPlayerDies();
    }
    private void CheckIfPlayerDies()
    {
        if (fuel <= 0 && currentState != State.DEAD)
        {
            fuel = 0;
            Die();
        }
    }
    private void Die()
    {
        ChangeState(State.DEAD);
        DisableScripts();
        CameraController.Instance.AddHighTrauma();
        Invoke("ExploteShip", timeToExploteShip);

        c_rb.drag /= 4;
        c_rb.angularDrag /= 4;

        shipLight.intensity /= 3;
        shipLight.pointLightInnerRadius /= 2;
        shipLight.pointLightOuterRadius /= 4;

        engineParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        AudioManager._instance.Play2dOneShotSound(enginePoweringOff, "Death", 1, 1, 1);
    }
    private void ExploteShip()
    {
        spriteRenderer.enabled = false;
        CameraController.Instance.AddHighTrauma();
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        InventoryManager.Instance.EndRun(false);
        shipLight.intensity = 0;
        Invoke("ReturnToHub", timeToReturnHub);
        AudioManager._instance.Play2dOneShotSound(deathExplosion, "Death", 1, 1, 1);
    }
    private void DisableScripts()
    {
        sizeUpgrade.enabled = false;
        GetComponent<UpgradeSelector>().enabled = false;
        mapInteraction.enabled = false;
    }
    private void ReturnToHub()
    {
        SceneManager.LoadScene("HubScene");
    }

    public void GetDamage(float value, Vector2 damagePos)
    {
        switch (currentState)
        {
            case State.MINING:
            case State.KNOCKBACK:
            case State.INVENCIBILITY:
            case State.FREEZE:
                return;
            default:
                break;
        }

        CameraController.Instance.AddHighTrauma();
        PlayHitParticles(damagePos);

        Knockback(damagePos);

        fuel -= value / PowerUpManager.Instance.Armor;


        if (OnHit != null)
            OnHit();
    }

    private void PlayHitParticles(Vector2 damagePos)
    {
        hitParticles.transform.forward = ((Vector3)damagePos - transform.position).normalized;
        hitParticles.Play();
    }

    public void SubstractFuel(float value)
    {
        fuel -= value;
    }
    public float GetFuel()
    {
        return fuel;
    }
    public float GetMaxFuel()
    {
        return baseFuel + PowerUpManager.Instance.Fuel;
    }

    #endregion

    #region States
    private void CheckIdleOrMovingState()
    {
        if (accelerationValue == 0 && currentState == State.MOVING)
        {
            ChangeState(State.IDLE);

        }
        else if (accelerationValue != 0 && currentState == State.IDLE)
        {
            ChangeState(State.MOVING);
        }
    }

    private void WaitForKnockbackTime()
    {
        ChangeState(State.MOVING);
    }

    private void Knockback(Vector2 collisionPoint)
    {
        ChangeState(State.KNOCKBACK);
        Vector2 direction = (Vector2)transform.position - collisionPoint;
        direction.Normalize();

        c_rb.AddForce(direction * knockbackScale * sizeUpgrade.sizeMultiplyer, ForceMode2D.Impulse);
        c_rb.AddTorque(UnityEngine.Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);

        Invoke("WaitForKnockbackTime", knockbackTime);
    }
    public void ChangeState(State state)
    {
        switch (currentState)
        {
            case State.IDLE:
                fuelConsume += idleFuelConsume;
                break;
            case State.MOVING:
                fuelConsume += movingFuelConsume;
                break;
            case State.MINING:
                //Cambiar al mapa de acciones normal del player
                InputController.Instance.ChangeActionMap("Player");
                mapInteraction.showCanvas = true;
                break;
            case State.KNOCKBACK:
                break; 
            case State.FREEZE:
                break;
            case State.DEAD:
                return;
            default:
                break;
        }

        switch (state)
        {
            case State.IDLE:
                fuelConsume -= idleFuelConsume;
                break;
            case State.MOVING:
                fuelConsume -= movingFuelConsume;
                break;
            case State.MINING:
                //Cambiar al mapa de acciones de minar
                InputController.Instance.ChangeActionMap("MinigameMinery");
                mapInteraction.showCanvas = false;
                break;
            case State.KNOCKBACK:
                c_rb.velocity = Vector2.zero;
                break;

            case State.FREEZE:
                engineParticles.gameObject.SetActive(false);
                c_rb.velocity = Vector2.zero;
                break;
            case State.DEAD:
                knockbackScale *= 2;
                StartCoroutine(AudioManager._instance.FadeOutSFXLoop(engineSource));
                break;
            default:
                break;
        }
        currentState = state;
    }
    public State GetState()
    {
        return currentState;
    }
    public float GetSpeed()
    {
        return movementSpeed;
    }

    #endregion

    #region Input
    private void RotateAction(InputAction.CallbackContext obj)
    {
        Vector2 currDirection = obj.action.ReadValue<Vector2>();

        if (currDirection != Vector2.zero)
        {
            movementDirection = currDirection;
        }
    }
    private void AccelerateAction(InputAction.CallbackContext obj)
    {
        if (obj.started && currentState == State.IDLE)
           engineSource = AudioManager._instance.Play2dLoop(engineClip, "Engine");

        if (obj.canceled && engineSource != null)
            StartCoroutine(AudioManager._instance.FadeOutSFXLoop(engineSource));

        accelerationValue = obj.ReadValue<float>();
    }

    private void DashAction(InputAction.CallbackContext obj)
    {
        if(canDash)
        {
            canDash = false;
            lastTimeDash = 0;
            Dash();
        }
    }

    #endregion

    private void PlayerPause()
    {
        c_rb.velocity = Vector2.zero;
        c_rb.angularVelocity = 0.0f;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map") || collision.collider.CompareTag("BreakableWall"))
        {
            GetDamage(mapDamage, collision.contacts[0].point);
            AudioManager._instance.Play2dOneShotSound(collisionClip, "Player");
        }

        if(collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            GetDamage(enemy.damage, collision.GetContact(0).point);
        }
    }

    public void StopEngineSource()
    {
        engineSource.Stop();
    }
}
