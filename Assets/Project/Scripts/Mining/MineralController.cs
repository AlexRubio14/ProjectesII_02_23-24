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
    private Vector2 originalBoxSize;
    private Vector2 originalBoxOffset;


    public float[] mineralsHealth {  get; private set; }
    [field: SerializeField]
    public float mineralRockBaseHealth {  get; private set; }

    public float currentRockHealth {  get; set; }

    private void Awake()
    {
        c_spriteR = GetComponent<SpriteRenderer>();
        c_boxCollider = GetComponent<BoxCollider2D>();

        if(isHide)
        {
            c_spriteR.sprite = hideSprite;
            originalBoxSize = c_boxCollider.size;
            originalBoxOffset = c_boxCollider.offset;
            c_boxCollider.size = Vector2.one * 0.1f;
            c_boxCollider.offset = originalBoxOffset;
        }
        else
        {
            if (isBeta)
                c_spriteR.sprite = c_currentItem.BetaSprite;
            else
                c_spriteR.sprite = c_currentItem.MapSprite;
        }

        currentRockHealth = mineralRockBaseHealth;
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

        mineralsHealth = new float[MaxItemsToReturn];
        for (int i = 0; i < MaxItemsToReturn; i++)
        {
            mineralsHealth[i] = c_currentItem.BaseMineralHealth;
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
        c_boxCollider.size = originalBoxSize;
        c_boxCollider.offset = originalBoxOffset;

        if (isBeta)
            c_spriteR.sprite = c_currentItem.BetaSprite;
        else
            c_spriteR.sprite = c_currentItem.MapSprite;

        isInteractable = true;
        isHide = false;

        SetupParticles(vfxUnhideColor);
    }
}
