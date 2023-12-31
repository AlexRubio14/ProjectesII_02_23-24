using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PickableItemController : MonoBehaviour
{
    [SerializeField]
    public ItemObject c_currentItem;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float minDistanceToGetItem;
    private bool playerThrow;

    [SerializeField]
    private AudioClip collectClip;

    private Transform c_playerTransform;
    private Rigidbody2D c_rb2d;
    private void Awake()
    {
        c_rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = c_currentItem.c_PickableSprite;
        GetComponentInChildren<Light2D>().color = c_currentItem.EffectsColor;

    }

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (c_playerTransform)
        {
            Vector2 dir = (c_playerTransform.position - transform.position).normalized;
            c_rb2d.AddForce(dir * moveSpeed, ForceMode2D.Force);

            CheckGetDistance();
        }
    }
    private void CheckGetDistance()
    {
        if (Vector2.Distance(c_playerTransform.position, transform.position) <= minDistanceToGetItem)
        {
            InventoryManager.Instance.ChangeRunItemAmount(c_currentItem, 1);
            AudioManager._instance.Play2dOneShotSound(collectClip, "Items");
            Destroy(gameObject);
        }
    }

    public void ImpulseItem(Vector2 _dir, float _throwSpeed)
    {
        c_rb2d.AddForce(_dir * _throwSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerThrow)
        {
            c_playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerThrow && collision.CompareTag("Player"))
        {
            playerThrow = false;
        }
    }
}
