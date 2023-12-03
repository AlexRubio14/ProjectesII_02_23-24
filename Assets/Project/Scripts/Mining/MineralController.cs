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
            SetupHiddenParticles();
        }

    }

    public override void Interact()
    {
        player.StartMinery(this);
    }
    public override void UnHide()
    {
        if (!isBeta)
        {
            //Borrar solo el del centro
            Vector3Int cellPos = grid.LocalToCell(transform.position);
            tilemap.SetTile(cellPos, null);
        }
        else
        {
            //Borrar los que estan en cada extremo del mineral
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            Vector3Int cellPos;

            cellPos = grid.LocalToCell(new Vector2(box.bounds.min.x, box.bounds.min.y));
            tilemap.SetTile(cellPos, null);

            cellPos = grid.LocalToCell(new Vector2(box.bounds.max.x, box.bounds.min.y));
            tilemap.SetTile(cellPos, null);

            cellPos = grid.LocalToCell(new Vector2(box.bounds.min.x, box.bounds.max.y));
            tilemap.SetTile(cellPos, null);

            cellPos = grid.LocalToCell(new Vector2(box.bounds.max.x, box.bounds.max.y));
            tilemap.SetTile(cellPos, null);
        }


        isInteractable = true;
        isHide = false;

        currentLight.enabled = true;
        currentHiddenParticles.Stop();

    }
}
