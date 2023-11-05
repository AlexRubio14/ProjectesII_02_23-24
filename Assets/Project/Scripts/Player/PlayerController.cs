using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public enum State { IDLE, MOVING, MINING, KNOCKBACK, INVENCIBILITY};
    private State currentState;

    private Rigidbody2D c_rb;

    [Space, Header("Movement"), SerializeField]
    private float movementScale;
    //private Vector3 inputDirection;

    [Header("Rotation")]
    private float currentRotationSpeed;
    [SerializeField]
    private float minRotationSpeed;
    [SerializeField]
    private float maxRotationSpeed;

    [Header("Knockback"), SerializeField]
    private float knockbackScale;
    [SerializeField]
    private float knockbackRotation;

    //[Header("Health"), SerializeField]
    [SerializeField]
    private float baseFuel;
    public float Fuel { get; private set; }
    [SerializeField]
    private float mapDamage;

    [Header("Storage"), SerializeField]
    private int maxStorage;
    private int currentStorage;

    private InputController iController;


    private void Awake()
    {
        currentState = State.MOVING;
        c_rb = GetComponent<Rigidbody2D>();

        currentRotationSpeed = minRotationSpeed;

        iController = GetComponentInParent<InputController>();
    }
    
    private void Start()
    {
        Fuel = baseFuel + PowerUpManager.Instance.Fuel;
    }

    void Update()
    {
        LoseFuel();
        RotationAcceleration();
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

    void Move()
    {
        c_rb.AddForce(transform.up * iController.inputMovementDirection.y * movementScale, ForceMode2D.Force);
    }

    void Rotation()
    {
        c_rb.AddTorque(currentRotationSpeed * (iController.inputMovementDirection.x * -1), ForceMode2D.Force); 
    }

    void RotationAcceleration()
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
    void LoseFuel()
    {
        switch (currentState)
        {
            case State.MOVING:
                if(iController.inputMovementDirection == Vector2.zero)
                {
                    Fuel -= Time.deltaTime / 3;
                }
                else
                {
                    Fuel -= Time.deltaTime;
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
    }
    public float GetHealth()
    {
        return Fuel;
    }
    private void CheckIfPlayerDies()
    {
        if (Fuel <= 0)
        {
            Fuel = 0;
            Die();
        }
    }
    void Die()
    {
        enabled = false;

        CameraController.Instance.AddHighTrauma();

        //SceneManager.LoadScene("HUB");
    }

    void Stunned()
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
    void Knockback(Vector2 collisionPoint)
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
        switch(currentState)
        {
            case State.MOVING:      
                break;
            case State.MINING:
                break;
            case State.KNOCKBACK:
                break;
            default:
                break;
        }

        switch (state)
        {
            case State.MOVING:
                break;
            case State.MINING:
                break;
            case State.KNOCKBACK:
                c_rb.velocity = Vector3.zero;
                break;
            default:
                break;
        }
        currentState = state;
    }

    public void GetDamage(float value, Vector2 damagePos)
    {
        CameraController.Instance.AddMediumTrauma();
        if (currentState != State.KNOCKBACK) 
            Fuel -= value / PowerUpManager.Instance.Armor;

        CheckIfPlayerDies();
        Knockback(damagePos);
    }

    void CheckStorage()
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

    public void SubstractHealth(float value)
    {
        Fuel -= value;
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
