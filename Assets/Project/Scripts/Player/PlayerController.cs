using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public enum State { MOVING, MINING, KNOCKBACK};
    private State currentState;

    private Rigidbody2D c_rb;

    //Movement
    private Vector3 inputDirection;
    [SerializeField]
    private float movementScale;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float knockbackScale;
    [SerializeField]
    private float knockbackRotation;
    [SerializeField]
    private bool toogleMovement;

    //Health
    [field: SerializeField]
    public float health {  get; private set; }
    [SerializeField]
    private float mapDamage;

    //Inventory
    [SerializeField]
    private int maxStorage;
    private int currentStorage;


    private void Awake()
    {
        currentState = State.MOVING;
        c_rb = GetComponent<Rigidbody2D>();

        maxStorage = 50;
    }
    

    // Update is called once per frame
    void Update()
    {
        GetDirectionFromInputs();
        LoseFuel();

        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            toogleMovement = !toogleMovement;
            if(toogleMovement) 
            {
                rotationSpeed = 300;
            }
            else
            {
                rotationSpeed = 7;
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
            default:
                break;
        }
    }

    void GetDirectionFromInputs()
    {
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");
        inputDirection.Normalize();
    }

    void Move()
    {
        if(toogleMovement)
        {
            c_rb.AddForce(transform.up * inputDirection.magnitude * movementScale, ForceMode2D.Force);
        }
        else
        {
            c_rb.AddForce(transform.up * inputDirection.y * movementScale, ForceMode2D.Force);
        }
    }

    void Rotation()
    {
        if(toogleMovement)
        {
            if (inputDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(transform.forward, inputDirection);
                Quaternion rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

                c_rb.MoveRotation(rotation);
            }
        }
        else
        {
            c_rb.AddTorque(rotationSpeed * (inputDirection.x * -1), ForceMode2D.Force);
            
        }
        
    }

    void LoseFuel()
    {
        health -= Time.deltaTime;
    }
    public float GetHealth()
    {
        return health;
    }

    void Die()
    {
        enabled = false;


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
        if(currentState != State.KNOCKBACK) 
            health -= value;

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
