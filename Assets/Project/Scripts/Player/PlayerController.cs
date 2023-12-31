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

    public enum State { IDLE, MOVING, MINING, KNOCKBACK, INVENCIBILITY, FREEZE, DEAD};
    private State currentState;

    private Rigidbody2D c_rb;
    [Space, Header("Movement"), SerializeField]
    private float movementSpeed;
    private float accelerationValue;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;
    [HideInInspector]
    public float externalMovementSpeed;



    public enum RotationType { OLD_ROTATION, NEW_ROTATION };
    [Space, Header("Rotation"), SerializeField]
    private RotationType currentRotation;
    [SerializeField]
    private float[] rotationSpeed;

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

    [Space, Header("Death"), SerializeField]
    private GameObject explosionParticles;
    [SerializeField]
    private float timeToExploteShip;
    [SerializeField]
    private float timeToReturnHub;
    private Light2D shipLight;
    [SerializeField]
    private ParticleSystem engineParticles;

    [Space, Header("Audio"), SerializeField]
    private AudioClip collisionClip;
    [SerializeField]
    private AudioClip engineClip;
    private AudioSource engineSource;

    private InputController inputController;
    private SpriteRenderer spriteRenderer;
    private PlayerMapInteraction c_mapInteraction;
    private AutoHelpController autoHelpController;
    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();

        inputController = GetComponent<InputController>();
        c_mapInteraction = GetComponent<PlayerMapInteraction>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shipLight = GetComponentInChildren<Light2D>();
        autoHelpController = GetComponent<AutoHelpController>();

    }
    
    private void Start()
    {
        fuelConsume = idleFuelConsume;
        currentState = State.IDLE;

        fuel = baseFuel + PowerUpManager.Instance.Fuel;
    }

    private void OnEnable()
    {
        rotationAction.action.started += RotateAction;
        rotationAction.action.performed += RotateAction;
        rotationAction.action.canceled += RotateAction;

        accelerateAction.action.started += AccelerateAction;
        accelerateAction.action.performed += AccelerateAction;
        accelerateAction.action.canceled += AccelerateAction;

    }

    private void OnDisable()
    {
        rotationAction.action.started -= RotateAction;
        rotationAction.action.performed -= RotateAction;
        rotationAction.action.canceled -= RotateAction;

        accelerateAction.action.started -= AccelerateAction;
        accelerateAction.action.performed -= AccelerateAction;
        accelerateAction.action.canceled -= AccelerateAction;
    }

    void Update()
    {        
        fuel = Mathf.Clamp(fuel, 0, Mathf.Infinity);

        if(Input.GetKey(KeyCode.L))
        {
            fuel += 50;
        }
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

        c_rb.AddForce(acceleration, ForceMode2D.Force);
    }
    private void Rotation()
    {
        if (currentRotation == RotationType.OLD_ROTATION)
        {
            Vector2 movementAndAutoHelpDirection;

            Vector2 autoHelp = autoHelpController.autoHelpDirection;
            float autoHelpMutliplier = 3;
            //En caso de que se tenga que aplicar la auto ayuda
            //Y el player no este tocando ningun input
            //Y la ultima direccion no este mirando directo a la pared
            //Se usara la ultima direccion de movimiento

            if (autoHelp != Vector2.zero && movementDirection == Vector2.zero && Vector2.Dot(lastMovementDirection, autoHelp) > -0.6f)
            {
                movementAndAutoHelpDirection = lastMovementDirection.normalized * autoHelpMutliplier + autoHelp;
            }
            else
            {
                movementAndAutoHelpDirection = movementDirection.normalized * autoHelpMutliplier + autoHelp;
            }

            Vector2 normalizedInputDirection = movementAndAutoHelpDirection.normalized;

            float signedAngle = Vector2.SignedAngle(transform.right, normalizedInputDirection);

            c_rb.AddTorque(signedAngle * rotationSpeed[(int)RotationType.OLD_ROTATION]);

        }
        else
        {
            Vector2 movementAndAutoHelpDirection = new Vector2(movementDirection.x, 0).normalized + autoHelpController.autoHelpDirection.normalized;

            float signedAngle = Vector2.SignedAngle(transform.right, movementAndAutoHelpDirection);

            c_rb.AddTorque(-movementDirection.x * rotationSpeed[(int)RotationType.NEW_ROTATION]);
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
        fuel = Mathf.Clamp(fuel - fuelConsume * Time.fixedDeltaTime, 0, GetMaxFuel());
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
        CameraController.Instance.AddHighTrauma();
        Invoke("ExploteShip", timeToExploteShip);

        c_rb.drag /= 4;
        c_rb.angularDrag /= 4;

        shipLight.intensity /= 3;
        shipLight.pointLightInnerRadius /= 2;
        shipLight.pointLightOuterRadius /= 4;

        engineParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    private void ExploteShip()
    {
        spriteRenderer.enabled = false;
        CameraController.Instance.AddHighTrauma();
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        InventoryManager.Instance.EndRun(false);
        shipLight.intensity = 0;
        Invoke("ReturnToHub", timeToReturnHub);
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
        Knockback(damagePos);
        fuel -= value / PowerUpManager.Instance.Armor;
    }
    public void SubstractHealth(float value)
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

        c_rb.AddForce(direction * knockbackScale, ForceMode2D.Impulse);
        c_rb.AddTorque(Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);

        Invoke("WaitForKnockbackTime", knockbackTime);
    }
    public void ChangeState(State state)
    {
        switch (currentState)
        {
            case State.IDLE:
                fuelConsume -= idleFuelConsume;
                break;
            case State.MOVING:
                fuelConsume -= movingFuelConsume;
                break;
            case State.MINING:
                //Cambiar al mapa de acciones normal del player
                inputController.ChangeActionMap("Player");
                c_mapInteraction.showCanvas = true;
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
                fuelConsume += idleFuelConsume;
                break;
            case State.MOVING:
                fuelConsume += movingFuelConsume;
                break;
            case State.MINING:
                //Cambiar al mapa de acciones de minar
                inputController.ChangeActionMap("MinigameMinery");
                c_mapInteraction.showCanvas = false;
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
        movementDirection = obj.action.ReadValue<Vector2>();

        if (movementDirection != Vector2.zero)
        {
            lastMovementDirection = movementDirection;
        }
    }
    private void AccelerateAction(InputAction.CallbackContext obj)
    {
        if (obj.started)
           engineSource = AudioManager._instance.Play2dLoop(engineClip, "Engine");

        if (obj.canceled)
            StartCoroutine(AudioManager._instance.FadeOutSFXLoop(engineSource));

        accelerationValue = obj.ReadValue<float>();
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map") || collision.collider.CompareTag("BreakableWall"))
        {
            AudioManager._instance.Play2dOneShotSound(collisionClip, "Player");
            GetDamage(mapDamage, collision.contacts[0].point);
        }

        if(collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            GetDamage(enemy.damage, collision.GetContact(0).point);
        }
    }
}
