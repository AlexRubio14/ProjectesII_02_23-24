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

    private Light2D currentLight;

    private PlayerMineryController player;

    private SpriteRenderer c_spriteR;
    private void Awake()
    {
        c_spriteR = GetComponent<SpriteRenderer>();
        c_spriteR.sprite = c_currentItem.c_MapSprite;
        currentLight = GetComponentInChildren<Light2D>();
        currentLight.color = c_currentItem.LightColor;
    }
    private void Start()
    {
        player = PlayerManager.Instance.player.gameObject.GetComponent<PlayerMineryController>();

        if (isHide)
        {
            currentLight.enabled = false;
            isInteractable = false;
        }

    }

    public override void Interact()
    {
        player.StartMinery(this);
    }
    public override void UnHide()
    {
        Vector3Int cellPos = grid.LocalToCell(transform.position);
        tilemap.SetTile(cellPos, null);
        currentLight.enabled = true;

        isInteractable = true;
        isHide = false;
    }
}
