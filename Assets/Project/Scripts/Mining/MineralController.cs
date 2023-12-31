using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MineralController : InteractableObject
{
    [field: Space, Header("Mineral"), SerializeField]
    public ItemObject c_currentItem { private set; get; }

    [field : SerializeField]
    public short MaxItemsToReturn { private set; get; }

    [SerializeField]
    private bool isBeta;

    [SerializeField]
    private Sprite hideSprite;  

    private PlayerMineryController player;

    private SpriteRenderer c_spriteR;

    private BoxCollider2D c_boxCollider;
    private void Awake()
    {
        c_spriteR = GetComponent<SpriteRenderer>();
        c_boxCollider = GetComponent<BoxCollider2D>();

        if(isHide)
        {
            c_spriteR.sprite = hideSprite;
            c_boxCollider.isTrigger = true;
        }
        else
        {
            if (isBeta)
                c_spriteR.sprite = c_currentItem.c_BetaSprite;
            else
                c_spriteR.sprite = c_currentItem.c_MapSprite;
        }
    }
    private void Start()
    {
        player = PlayerManager.Instance.player.gameObject.GetComponent<PlayerMineryController>();

        vfxUnhideColor = new Color(c_currentItem.EffectsColor.r, c_currentItem.EffectsColor.g, c_currentItem.EffectsColor.b, 0.1f);

        if (isHide)
        {
            isInteractable = false;
            SetupParticles(vfxHideColor);
        }
        else
        {
            SetupParticles(vfxUnhideColor);
        }

    }

    public override void Interact()
    {
        player.StartMinery(this);
    }
    public override void UnHide()
    {
        base.UnHide();
        c_boxCollider.isTrigger = false;

        if (isBeta)
            c_spriteR.sprite = c_currentItem.c_BetaSprite;
        else
            c_spriteR.sprite = c_currentItem.c_MapSprite;

        isInteractable = true;
        isHide = false;

        SetupParticles(vfxUnhideColor);
    }
}
