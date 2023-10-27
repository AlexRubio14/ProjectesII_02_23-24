using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private enum State { IDLE, MOVING, MINING};

    private Rigidbody2D c_rb;

    //Movement
    private Vector3 inputDirection;
    [SerializeField]
    private float movementScale;
    [SerializeField]
    private float rotationSpeed;

    [field: SerializeField]
    public float health {  get; private set; }


    // Start is called before the first frame update
    void Start()
    {
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
