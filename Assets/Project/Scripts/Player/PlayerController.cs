using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public enum State { IDLE, MOVING, MINING, KNOCKBACK, INVENCIBILITY, FREEZE, DEAD};
    private State currentState;

    private Rigidbody2D c_rb;

    [Header("Movement"), SerializeField]
    private float movementScale;
    //private Vector3 inputDirection;

    [Space, Header("Rotation")]
    private float currentRotationSpeed;
    [SerializeField]
    private float minRotationSpeed;
    [SerializeField]
    private float maxRotationSpeed;

    [Space, Header("Knockback"), SerializeField]
    private float knockbackScale;
    [SerializeField]
    private float knockbackRotation;

    [Space, Header("Health"), SerializeField]
    private float baseFuel;
    public float Fuel { get; private set; }
    [SerializeField]
    private float mapDamage;
    [Header("Death"), SerializeField]
    private GameObject explosionParticles;
    [SerializeField]
    private float timeToExploteShip;
    [SerializeField]
    private float timeToReturnHub;
    private Light2D shipLight;
    [SerializeField]
    private ParticleSystem engineParticles;

    [Space, Header("Storage"), SerializeField]
    private int maxStorage;
    private int currentStorage;

    private InputController iController;
    private SpriteRenderer spriteRenderer;
    private PlayerMapInteraction c_mapInteraction;

    private void Awake()
    {
        currentState = State.MOVING;
        c_rb = GetComponent<Rigidbody2D>();

        currentRotationSpeed = minRotationSpeed;

        iController = GetComponentInParent<InputController>();
        c_mapInteraction = GetComponent<PlayerMapInteraction>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shipLight = GetComponentInChildren<Light2D>();

    }
    
    private void Start()
    {
        Fuel = baseFuel + PowerUpManager.Instance.Fuel;
    }

    void Update()
    {
        LoseFuel();
        RotationAcceleration();

        Fuel = Mathf.Clamp(Fuel, 0, Mathf.Infinity);
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
            default:
                break;
        }
    }

    #region Movement
    private void Move()
    {
        c_rb.AddForce(transform.up * iController.inputMovementDirection.y * movementScale, ForceMode2D.Force);
    }
    private void Rotation()
    {
        c_rb.AddTorque(currentRotationSpeed * (iController.inputMovementDirection.x * -1), ForceMode2D.Force); 
    }
    private void RotationAcceleration()
    {
        if (iController.inputMovementDirection.x != 0)
        {
            if (currentRotationSpeed <= maxRotationSpeed)
            {
                currentRotationSpeed += Time.deltaTime;
            }
            
        }
        else
        {
            if(currentRotationSpeed >= minRotationSpeed)
            {
                currentRotationSpeed -= Time.deltaTime;
            }
        }
    }
    #endregion

    #region Ship Health & Fuel
    void LoseFuel()
    {
        switch (currentState)
        {
            case State.MOVING:
                if(iController.inputMovementDirection == Vector2.zero)
                {
                    Fuel -= Time.deltaTime / 3;
                    
                    if(engineParticles.isPlaying)
                        engineParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                }
                else
                {
                    Fuel -= Time.deltaTime;

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
            default:
                break;
        }

        if(Time.deltaTime / 3 == 0)
        {
            Fuel -= Time.deltaTime;
        }

        CheckIfPlayerDies();
    }
    private void CheckIfPlayerDies()
    {
        if (Fuel <= 0 && currentState != State.DEAD)
        {
            Fuel = 0;
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
        Fuel -= value / PowerUpManager.Instance.Armor;
    }
    public void SubstractHealth(float value)
    {
        Fuel -= value;
    }
    public float GetFuel()
    {
        return Fuel;
    }

    #endregion

    #region States
    private void Stunned()
    {
        if (c_rb.velocity.magnitude <= 0.1f)
        {
            ChangeState(State.MOVING);
        }
        else
        {
            Invoke("Stunned", 0.02f);
        }
    }
    private void Knockback(Vector2 collisionPoint)
    {
        ChangeState(State.KNOCKBACK);
        Vector2 direction = (Vector2)transform.position - collisionPoint;
        direction.Normalize();

        c_rb.AddForce(direction * knockbackScale, ForceMode2D.Impulse);
        c_rb.AddTorque(Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);

        Stunned();
    }

    public void ChangeState(State state)
    {
        switch (currentState)
        {
            case State.MOVING:
                break;
            case State.MINING:
                //Cambiar al mapa de acciones normal del player
                iController.ChangeActionMap("Player");
                c_mapInteraction.showCanvas = true;
                break;
            case State.KNOCKBACK:
                break;
            case State.FREEZE:
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
                iController.ChangeActionMap("MinigameMinery");
                c_mapInteraction.showCanvas = false;
                break;
            case State.KNOCKBACK:
                c_rb.velocity = Vector2.zero;
                break;
            case State.FREEZE:
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

    #endregion

    #region Getters & Setters
   
    public State GetState()
    {
        return currentState;
    }
    #endregion

    private void CheckStorage()
    {
        if(currentStorage <= maxStorage * 0.5f)
        {

        }
        else if(currentStorage <= maxStorage * 0.75f)
        {

        }
        else
        {

        }
            
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map"))
        {
            GetDamage(mapDamage, collision.contacts[0].point);
        }

        if(collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            GetDamage(enemy.GetDamage(), collision.GetContact(0).point);
        }
    }
}
