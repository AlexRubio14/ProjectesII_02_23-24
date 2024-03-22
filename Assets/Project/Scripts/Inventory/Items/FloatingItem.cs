using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
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
        if (Vector2.Distance(PlayerManager.Instance.player.transform.position, transform.position) <= minDistanceToGetItem)
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
