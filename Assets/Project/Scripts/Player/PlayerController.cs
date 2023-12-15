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
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference accelerateAction;

    public enum State { IDLE, MOVING, MINING, KNOCKBACK, DRILL, INVENCIBILITY, FREEZE, BOOST, DEAD};
    private State currentState;

    private Rigidbody2D c_rb;
    [Space, Header("Movement"), SerializeField]
    private float movementScale;
    private float currentMovementScale;
    private float accelerationValue;
    private Vector2 movementDirection;

    [Space, Header("Rotation")]
    [SerializeField]
    private float minRotationSpeed;
    [SerializeField]
    private float maxRotationSpeed;
    [SerializeField]
    private float rotationSpeed;


    [Space, Header("Knockback"), SerializeField]
    private float knockbackScale;
    [SerializeField]
    private float knockbackRotation;
    [SerializeField]
    private float knockbackTime;

    [Space, Header("Health"), SerializeField]
    private float baseFuel;
    public float fuel { get; private set; }
    [SerializeField]
    private float mapDamage;

    [Space, Header("Death"), SerializeField]
    private GameObject explosionParticles;
    [SerializeField]
    private float timeToExploteShip;
    [SerializeField]
    private float timeToReturnHub;
    private Light2D shipLight;
    [SerializeField]
    private ParticleSystem engineParticles;

    [Space, Header("Drill"), SerializeField]
    private float drillMovementScale;
    [SerializeField]
    private GameObject drillSprite;
    private DrillController drillController;

    [Space, Header("Boost"), SerializeField]
    private float boostMovementScale;
    [SerializeField]
    private float boostMovementWithoutAcceleration;

    [Space, Header("Storage"), SerializeField]
    private int maxStorage;
    private int currentStorage;

    private InputController inputController;
    private SpriteRenderer spriteRenderer;
    private PlayerMapInteraction c_mapInteraction;
    private WebController webController;


    private void Awake()
    {
        currentState = State.MOVING;
        c_rb = GetComponent<Rigidbody2D>();

        inputController = GetComponent<InputController>();
        c_mapInteraction = GetComponent<PlayerMapInteraction>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        drillController = GetComponent<DrillController>();
        shipLight = GetComponentInChildren<Light2D>();
        webController = GetComponent<WebController>();
    }
    
    private void Start()
    {
        fuel = baseFuel + PowerUpManager.Instance.Fuel;
        currentMovementScale = movementScale;
    }

    private void OnEnable()
    {
        moveAction.action.started += MoveAction;
        moveAction.action.performed += MoveAction;
        moveAction.action.canceled += MoveAction;

        accelerateAction.action.started += AccelerateAction;
        accelerateAction.action.performed += AccelerateAction;
        accelerateAction.action.canceled += AccelerateAction;

    }

    private void OnDisable()
    {
        moveAction.action.started -= MoveAction;
        moveAction.action.performed -= MoveAction;
        moveAction.action.canceled -= MoveAction;

        accelerateAction.action.started -= AccelerateAction;
        accelerateAction.action.performed -= AccelerateAction;
        accelerateAction.action.canceled -= AccelerateAction;
    }

    void Update()
    {
        LoseFuel();
        
        fuel = Mathf.Clamp(fuel, 0, Mathf.Infinity);
        if(Input.GetKey(KeyCode.L))
        {
            fuel += 50;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentState == State.DRILL)
            {
                ChangeState(State.MOVING);
            }else if (currentState == State.MOVING)
            {
                ChangeState(State.DRILL);
            }
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.MOVING:
                Move();
                Rotation();
                break;
            case State.MINING:
                break;
            case State.KNOCKBACK:
                break;
            case State.DRILL:
                Move();
                Rotation();
                break;
            case State.BOOST:
                BoostMove();
                Rotation();
                break;
            default:
                break;
        }
    }

    #region Movement
    private void Move()
    {
        Vector2 acceleration = transform.right * accelerationValue * currentMovementScale * webController.webDecrease;

        c_rb.AddForce(acceleration, ForceMode2D.Force);
    }

    private void BoostMove()
    {
        if(movementDirection == Vector2.zero)
        {
            c_rb.AddForce(transform.right * boostMovementWithoutAcceleration, ForceMode2D.Force);
        }
        else
        {
            Move();
        }
    }
 
    private void Rotation()
    {
        if (movementDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector2 normalizedInputDirection = movementDirection.normalized;

        Quaternion targetRotation = Quaternion.AngleAxis(
            Mathf.Clamp(Vector2.SignedAngle(transform.right, normalizedInputDirection), -rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime)
            , Vector3.forward);

        c_rb.SetRotation(transform.rotation * targetRotation);
    }
   
    #endregion

    #region Ship Health & Fuel
    void LoseFuel()
    {
        switch (currentState)
        {
            case State.MOVING:
                if(accelerationValue == 0)
                {
                    fuel -= Time.deltaTime / 3;
                    
                    if(engineParticles.isPlaying)
                        engineParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    fuel -= Time.deltaTime;

                    if (engineParticles.isStopped)
                        engineParticles.Play(true);
                }
                break;
            case State.MINING:
                break;
            case State.KNOCKBACK:
                break;
            case State.INVENCIBILITY:
                break;
            case State.BOOST:
                if(accelerationValue == 0)
                {
                    fuel -= Time.deltaTime;
                }
                else
                {
                    fuel -= Time.deltaTime * 2;
                }
                break;
            default:
                break;
        }

        if(Time.deltaTime / 3 == 0)
        {
            fuel -= Time.deltaTime;
        }

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

        CameraController.Instance.AddMediumTrauma();
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
            case State.MOVING:
                break;
            case State.MINING:
                //Cambiar al mapa de acciones normal del player
                inputController.ChangeActionMap("Player");
                c_mapInteraction.showCanvas = true;
                break;
            case State.KNOCKBACK:
                break; 
            case State.DRILL:
                currentMovementScale = movementScale;
                drillController.enabled = false;
                drillSprite.SetActive(false);
                c_rb.angularVelocity = 0;
                break;
            case State.FREEZE:
                break;
            case State.BOOST:
                currentMovementScale = movementScale;
                break;
            case State.DEAD:
                return;
            default:
                break;
        }

        switch (state)
        {
            case State.MOVING:
                break;
            case State.MINING:
                //Cambiar al mapa de acciones de minar
                inputController.ChangeActionMap("MinigameMinery");
                c_mapInteraction.showCanvas = false;
                break;
            case State.KNOCKBACK:
                c_rb.velocity = Vector2.zero;
                break;
            case State.DRILL:
                currentMovementScale = drillMovementScale;
                drillController.enabled = true;
                drillSprite.SetActive(true);

                break;
            case State.FREEZE:
                c_rb.velocity = Vector2.zero;
                break;
            case State.DEAD:
                knockbackScale *= 2;
                break;
                case State.BOOST:
                currentMovementScale = boostMovementScale;
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
        return movementScale;
    }

    public void SetSpeed(float value)
    {
        currentMovementScale = value;
    }

    #endregion

    #region Input
    private void MoveAction(InputAction.CallbackContext obj)
    {
        movementDirection = obj.action.ReadValue<Vector2>();
    }
    private void AccelerateAction(InputAction.CallbackContext obj)
    {
        accelerationValue = obj.ReadValue<float>();
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map") || collision.collider.CompareTag("BreakableWall"))
        {
            GetDamage(mapDamage, collision.contacts[0].point);
        }

        if(collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            GetDamage(enemy.damage, collision.GetContact(0).point);
        }
    }
}
