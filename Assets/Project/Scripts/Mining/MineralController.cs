using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MineralController : InteractableObject
{
    [field: Space, Header("Mineral"), SerializeField]
    public ItemObject currentItem { private set; get; }

    [field : SerializeField]
    public short MaxItemsToReturn { private set; get; }

    [SerializeField]
    private bool isBeta;

    [SerializeField]
    private Sprite hideSprite;  

    private PlayerMineryController player;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteRenderer fog;

    private BoxCollider2D boxCollider;
    private Vector2 originalBoxSize;
    private Vector2 originalBoxOffset;


    public float[] mineralsHealth {  get; private set; }
    [field: SerializeField]
    public float mineralRockBaseHealth {  get; private set; }

    public float currentRockHealth {  get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if(isHide)
        {
            spriteRenderer.sprite = hideSprite;
            originalBoxSize = boxCollider.size;
            originalBoxOffset = boxCollider.offset;
            boxCollider.size = Vector2.one * 0.1f;
            boxCollider.offset = originalBoxOffset;
        }
        else if(spriteRenderer)
        {
            if (isBeta)
                spriteRenderer.sprite = currentItem.BetaSprite;
            else
                spriteRenderer.sprite = currentItem.MapSprite;
        }

        currentRockHealth = mineralRockBaseHealth;
    }
    private void Start()
    {
        player = PlayerManager.Instance.player.gameObject.GetComponent<PlayerMineryController>();

        
        vfxUnhideColor = new Color(currentItem.EffectsColor.r, currentItem.EffectsColor.g, currentItem.EffectsColor.b, 0.1f);

        if (isHide)
        {
            isInteractable = false;
            SetupParticles(vfxHideColor);
        }
        else
        {
            SetupParticles(vfxUnhideColor);
            SetupFogAndLight();
        }

        mineralsHealth = new float[MaxItemsToReturn];
        for (int i = 0; i < MaxItemsToReturn; i++)
        {
            mineralsHealth[i] = currentItem.BaseMineralHealth;
        }

    }

    public override void Interact()
    {
        //player.StartNewMinery(this);
        player.StartMinery(this);
    }
    public override void UnHide()
    {
        base.UnHide();
        boxCollider.size = originalBoxSize;
        boxCollider.offset = originalBoxOffset;

        if (isBeta)
            spriteRenderer.sprite = currentItem.BetaSprite;
        else
            spriteRenderer.sprite = currentItem.MapSprite;

        isInteractable = true;
        isHide = false;

        SetupParticles(vfxUnhideColor);

        SetupFogAndLight();
    }


    private void SetupFogAndLight()
    {
        fog.color =  new Color (currentItem.EffectsColor.r, currentItem.EffectsColor.g, currentItem.EffectsColor.b, 0.05f);

        GetComponentInChildren<Light2D>().color = currentItem.EffectsColor;
    }
}
