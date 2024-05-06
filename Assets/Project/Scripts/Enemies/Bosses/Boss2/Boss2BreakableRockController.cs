using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss2BreakableRockController : MonoBehaviour
{
    public Tilemap tilemap {  get; private set; }
    public Rigidbody2D rb2d {  get; private set; }

    private TilemapCollider2D tilemapCollider;

    private Vector2 startPos;

    [SerializeField]
    private float maxDistanceFromStartPos;

    [Space, SerializeField]
    private Tile defaultTile;
    [SerializeField]
    private int rockSize;
    [SerializeField]
    private int rockBorderValue;


    [Space, SerializeField]
    private GameObject bubble;
    private bool canThrowBubble;
    [SerializeField]
    private bool checkDistance;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        rb2d = GetComponent<Rigidbody2D>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }

    private void OnEnable()
    {
        startPos = transform.position;
        canThrowBubble = true;
    }
    private void FixedUpdate()
    {
        CheckIfThrowBubble();
        CheckDistance();
    }

    private void CheckIfThrowBubble()
    {
        if (!canThrowBubble)
            return;

        if (!tilemap.GetTile(Vector3Int.zero))
        {
            //Crear Burbuja
            Instantiate(bubble, transform.position, Quaternion.identity);
            canThrowBubble = false;
        }
    }
    private void CheckDistance()
    {
        if (!checkDistance)
            return;

        float distance = Vector2.Distance(transform.position, startPos);
        if (distance >= maxDistanceFromStartPos)
            gameObject.SetActive(false);
    }
    public void ResetRockSize()
    {
        for (int i = -rockSize; i < rockSize + 1; i++)
        {
            for (int j = -rockSize; j < rockSize + 1; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) < rockBorderValue)
                {
                    //Puedo poner trozo de piedra
                    Vector3Int tilePos = new Vector3Int(i, j, 0);
                    tilemap.SetTile(tilePos, defaultTile);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BreakableWall"))
        {
            Boss2BreakableRockController rockController = collision.gameObject.GetComponent<Boss2BreakableRockController>();
            if (rockController)
                return;

            Physics2D.IgnoreCollision(collision.collider, tilemapCollider);
            
        }
    }
}
