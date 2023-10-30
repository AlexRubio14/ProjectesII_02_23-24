using System;
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

    [field: SerializeField]
    public float health {  get; private set; }

    public Action myActions;
    public Action<int> intActions;

    private void SayHi(int value)
    {
        Debug.Log("HI UwU");
    }

    private void Awake()
    {
        currentState = State.MOVING;
        c_rb = GetComponent<Rigidbody2D>();

        intActions += SayHi;
    }
    

    // Update is called once per frame
    void Update()
    {
        GetDirectionFromInputs();
        LoseFuel();

        if (health <= 0)
        {
            Die();
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
        c_rb.AddForce(transform.up * inputDirection.magnitude * movementScale, ForceMode2D.Force);
    }

    void Rotation()
    {
        if(inputDirection != Vector3.zero) 
        {
            Quaternion toRotation = Quaternion.LookRotation(transform.forward, inputDirection);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            c_rb.MoveRotation(rotation);
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
        Destroy(gameObject);

        //SceneManager.LoadScene("HUB");
    }

    void Stunned()
    {
        if (c_rb.velocity.magnitude <= 0.001f)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map"))
        {
            Knockback(collision.contacts[0].point);
        }
    }
}
