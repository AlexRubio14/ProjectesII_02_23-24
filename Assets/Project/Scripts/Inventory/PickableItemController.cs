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

    private Transform c_playerTransform;
    private Rigidbody2D c_rb2d;
    private void Awake()
    {
        c_rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = c_currentItem.c_pickableSprite;
        gameObject.AddComponent<BoxCollider2D>();
        GetComponentInChildren<Light2D>().color = c_currentItem.lightColor;

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
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            c_playerTransform = collision.transform;
        }
    }

    private void OnDestroy()
    {
        
    }

}
