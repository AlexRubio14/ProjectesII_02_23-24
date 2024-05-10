using UnityEngine;

public class FuelZoneItemController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    private float lerpProcess;
    private Vector2 startPos;
    private Vector2 endPos;
    private SpriteRenderer spriteRenderer;

    [Space, SerializeField]    
    private FuelZoneSaveItemsController fuelZoneItemsController;


    [SerializeField]
    private AudioClip tpItemClip;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        MoveToTP();
    }

    private void MoveToTP()
    {
        lerpProcess += Time.deltaTime * moveSpeed;
        transform.position = Vector2.Lerp(startPos, endPos, lerpProcess);

        if (lerpProcess > 1)
        {
            EnterTP();
        }
    }

    private void EnterTP()
    {
        lerpProcess = 0;
        //Play a las particulas
        fuelZoneItemsController.GetUnusedParticles().Play(true);
        AudioManager.instance.Play2dOneShotSound(tpItemClip, "TpInteraction", 0.6f, 0.6f, 1.6f);
        gameObject.SetActive(false);
    }


    public void Initialize(Vector2 _startPos, Vector2 _endPos, Sprite _itemSprite)
    {
        startPos = _startPos;
        endPos = _endPos;
        spriteRenderer.sprite = _itemSprite;
        lerpProcess = 0;
    }
}
