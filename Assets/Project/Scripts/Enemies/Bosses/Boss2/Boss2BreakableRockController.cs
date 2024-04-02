using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss2BreakableRockController : MonoBehaviour
{
    public Tilemap tilemap {  get; private set; }
    public Rigidbody2D rb2d {  get; private set; }


    private Vector2 startPos;
    [SerializeField]
    private float maxDistanceFromStartPos;

    [SerializeField]
    private GameObject bubble;
    private bool canThrowBubble;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        rb2d = GetComponent<Rigidbody2D>();
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
        float distance = Vector2.Distance(transform.position, startPos);
        if (distance >= maxDistanceFromStartPos)
            gameObject.SetActive(false);
    }
}
