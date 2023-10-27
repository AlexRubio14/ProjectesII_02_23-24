using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private enum State { MOVING, MINING, KNOCKBACK};
    private State currentState;

    private Rigidbody2D c_rb;

    //Movement
    private Vector3 inputDirection;
    [SerializeField]
    private float movementScale;
    [SerializeField]
    private float rotationSpeed;

    [field: SerializeField]
    public float health {  get; private set; }

    private void Awake()
    {
        currentState = State.MOVING;
        c_rb = GetComponent<Rigidbody2D>();
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


        Move();
        Rotation();
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
}
