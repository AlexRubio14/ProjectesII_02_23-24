using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class FloatingItem : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected float minDistanceToGetItem;

    protected bool canChase;

    [SerializeField]
    protected AudioClip collectClip;

    protected Rigidbody2D rb2d;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Light2D light2D;

    protected void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
    }
    protected void FixedUpdate()
    {
        FollowPlayer();
    }

    protected void FollowPlayer()
    {
        if (canChase)
            ChaseAction();
    }
    protected void CheckGetDistance(Transform _target)
    {
        if (Vector2.Distance(_target.position, transform.position) <= minDistanceToGetItem)
        {
            ObtainAction();
        }
    }
    protected abstract void ChaseAction();
    protected abstract void ObtainAction();
    public void ImpulseItem(Vector2 _dir, float _throwSpeed)
    {
        rb2d.AddForce(_dir * _throwSpeed, ForceMode2D.Impulse);
    }

}
