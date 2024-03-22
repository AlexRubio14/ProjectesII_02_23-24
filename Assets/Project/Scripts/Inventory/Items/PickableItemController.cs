using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PickableItemController : FloatingItem
{
    [Space, Header("PickableItem"), SerializeField]
    public ItemObject c_currentItem;

    [HideInInspector]
    public bool followPlayer = true;

    

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = c_currentItem.PickableSprite;
        GetComponentInChildren<Light2D>().color = c_currentItem.EffectsColor;
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
        InventoryManager.Instance.ChangeRunItemAmount(c_currentItem, 1);
        AudioManager._instance.Play2dOneShotSound(collectClip, "Items");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            canChase = true;
    }
}
