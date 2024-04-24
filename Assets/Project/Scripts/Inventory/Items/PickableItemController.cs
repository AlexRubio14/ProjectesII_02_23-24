using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PickableItemController : FloatingItem
{
    [Space, Header("PickableItem"), SerializeField]
    public ItemObject currentItem;

    [HideInInspector]
    public bool followPlayer = true;

    public Action onItemPicked;


    public void InitializeItem(ItemObject _currentItem)
    {
        currentItem = _currentItem;
        spriteRenderer.sprite = currentItem.PickableSprite;
        if (!light2D)
            light2D = GetComponentInChildren<Light2D>();
        light2D.color = currentItem.EffectsColor;
    }

    protected override void ChaseAction()
    {
        if (!followPlayer)
            return;
        Vector2 dir = (PlayerManager.Instance.player.transform.position - transform.position).normalized;
        rb2d.AddForce(dir * moveSpeed, ForceMode2D.Force);

        CheckGetDistance(PlayerManager.Instance.player.transform);
    }
    protected override void ObtainAction()
    {
        if (onItemPicked != null)
            onItemPicked();
        InventoryManager.Instance.ChangeRunItemAmount(currentItem, 1);
        AudioManager.instance.Play2dOneShotSound(collectClip, "Items");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("Collision");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            canChase = true;
    }
}
