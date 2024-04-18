using UnityEngine;

public class FuelZoneItemController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    private float lerpProcess;
    private Vector2 startPos;
    private Vector2 endPos;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        lerpProcess += Time.deltaTime * moveSpeed;
        transform.position = Vector2.Lerp(startPos, endPos, lerpProcess);

        if (lerpProcess > 1)
        {
            lerpProcess = 0;
            gameObject.SetActive(false);
        }
    }

    public void Initialize(Vector2 _startPos, Vector2 _endPos, Sprite _itemSprite)
    {
        startPos = _startPos;
        endPos = _endPos;
        spriteRenderer.sprite = _itemSprite;
        lerpProcess = 0;
    }
}
