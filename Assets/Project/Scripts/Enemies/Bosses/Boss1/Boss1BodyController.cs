using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Boss1BodyController : MonoBehaviour
{

    [SerializeField]
    private bool isHead;
    [SerializeField]
    private Transform head;
    private Rigidbody2D headRb;

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

    [SerializeField]
    private ParticleSystem explosionParticle;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;

    private Boss1Controller mainBossController;

    [Space, SerializeField]
    private Color hitColor;
    [SerializeField]
    private float hitColorLerpSpeed;
    private float hitColorLerpProcess;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        headRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainBossController = GetComponentInParent<Boss1Controller>();

        currentMovementSpeed = baseMovementSpeed;
        hitColorLerpProcess = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChaseTarget();
        OrientateSprite();
        CheckHitColor();
    }


    private void ChaseTarget()
    {
        if (!target)
            return;

        Vector3 targetPos = target.localPosition - target.right * offset;
        float externalForce = Mathf.Cos((Time.time % 360) * shakeSpeed) * forceApplyed;
        rb2d.velocity = (Vector2)(targetPos - transform.localPosition) * currentMovementSpeed + forceDirection * externalForce;
        forceApplyed = Mathf.Clamp(forceApplyed - Time.fixedDeltaTime * shakeReduction, 0, Mathf.Infinity);
        rb2d.angularVelocity = Quaternion.Angle(transform.rotation, target.rotation) * rotationSpeed;
        transform.rotation = target.rotation;


    }
    private void OrientateSprite()
    {
        if (rb2d.constraints == RigidbodyConstraints2D.FreezeAll)
            return;

        float dot = Vector2.Dot(headRb.velocity, Vector2.right);
        if (dot > 0)
            spriteRenderer.flipY = false;
        else if ( dot < 0)
            spriteRenderer.flipY = true;
        
    }
    private void CheckHitColor()
    {
        if (hitColorLerpProcess >= 1)
            return;

        hitColorLerpProcess += Time.fixedDeltaTime * hitColorLerpSpeed;

        spriteRenderer.color = Color.Lerp(hitColor, Color.white, hitColorLerpProcess);

        hitColorLerpProcess = Mathf.Clamp01(hitColorLerpProcess);
        
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

    public void SetFreeze(RigidbodyConstraints2D _freeze)
    {
        rb2d.constraints = _freeze;
    }


    public void ExplodeBodyPart()
    {
        explosionParticle.Play();
        spriteRenderer.enabled = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Map"))
            mainBossController.ChangeSpinDirection(collision, transform.position);

        if (collision.collider.CompareTag("Bullet"))
        {
            mainBossController.GetDamage(collision.gameObject.GetComponent<Laser>().GetBulletDamage());
            forceDirection = (transform.position - collision.transform.position).normalized;
            forceApplyed = shakeForce;
            hitColorLerpProcess = 0;
        }
    }

}
