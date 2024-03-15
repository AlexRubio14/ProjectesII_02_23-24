using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Boss1BodyController : MonoBehaviour
{
    [SerializeField]
    private Transform head;

    [Space, SerializeField]
    private Transform target;
    [SerializeField] 
    private float offset;
    [SerializeField]
    private float baseMovementSpeed;
    [SerializeField]
    private float spinMovementSpeed;
    private float currentMovementSpeed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float shakeForce;
    [SerializeField]
    private float shakeSpeed;
    [SerializeField]
    private float shakeReduction;
    private float forceApplyed;
    private Vector2 forceDirection;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;

    private Boss1Controller mainBossController;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainBossController = GetComponentInParent<Boss1Controller>();

        currentMovementSpeed = baseMovementSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChaseTarget();
        //OrientateSprite();
    }


    private void ChaseTarget()
    {
        if (!target)
            return;

        Vector3 targetPos = target.localPosition - target.right * offset;
        rb2d.velocity = (Vector2)(targetPos - transform.localPosition) * currentMovementSpeed + forceDirection * (Mathf.Cos(Time.time * shakeSpeed) * forceApplyed);
        forceApplyed = Mathf.Clamp(forceApplyed - Time.deltaTime * shakeReduction, 0, Mathf.Infinity)  ;
        rb2d.angularVelocity = Quaternion.Angle(transform.rotation, target.rotation) * rotationSpeed;
        transform.rotation = target.rotation;


    }
    private void OrientateSprite()
    {
        if (Vector2.Dot(head.right, Vector2.right) > 0)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }
        else
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }
    }

    public void ResetTailPos()
    {
        transform.localPosition = target.localPosition - target.right * offset;
    }

    public void SetLowFollowSpeed()
    {
        currentMovementSpeed = spinMovementSpeed;
    }
    public void SetNormalFollowSpeed()
    {
        currentMovementSpeed = baseMovementSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Map"))
        {
            mainBossController.ChangeSpinDirection(collision.contacts[0].normal);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            mainBossController.GetDamage(collision.GetComponent<Laser>().GetBulletDamage());
            forceDirection = (transform.position - collision.transform.position).normalized;
            forceApplyed = shakeForce;
        }
    }

}
