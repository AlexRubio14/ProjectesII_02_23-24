using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class DrillController : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private float drillDistance;
    [SerializeField]
    private float maxDrillAngle;
    [SerializeField]
    private int raysPerSide;
    
    [SerializeField]
    private LayerMask breakableWallLayer;

    List<RaycastHit2D> hits;

    [Space, SerializeField]
    private TileBase aloneTile;
    [SerializeField]
    private TileBase oneSideTileUp;
    [SerializeField]
    private TileBase oneSideTileDown;
    [SerializeField]
    private TileBase oneSideTileRight;
    [SerializeField]
    private TileBase oneSideTileLeft;
    [SerializeField]
    private TileBase twoSideTileDownLeft;
    [SerializeField]
    private TileBase twoSideTileDownRight;
    [SerializeField]
    private TileBase twoSideTileUpLeft;
    [SerializeField]
    private TileBase twoSideTileUpRight;
    [SerializeField]
    private TileBase twoSideTileUpDown;
    [SerializeField]
    private TileBase twoSideTileRightLeft;
    [SerializeField]
    private TileBase threeSideTileUp;
    [SerializeField]
    private TileBase threeSideTileDown;
    [SerializeField]
    private TileBase threeSideTileRight;
    [SerializeField]
    private TileBase threeSideTileLeft;


    private void Start()
    {
        hits = new List<RaycastHit2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {
            float range = drillDistance - (i / 90); //Esta resta se hace para que los rayos no esten todos a la misma distancia como su fuera un circulo
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            hits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.up).normalized, range, breakableWallLayer));
            if (i != 0)
            {
                direction = transform.rotation * Quaternion.Euler(0, 0, -i);
                hits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.up).normalized, range, breakableWallLayer));
            }
        }


        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                Vector3Int tilePos = grid.LocalToCell(hit.centroid);
                tilemap.SetTile(tilePos, null);

                UpdateSideTile(new Vector3Int(tilePos.x + 1, tilePos.y));
                UpdateSideTile(new Vector3Int(tilePos.x - 1, tilePos.y));
                UpdateSideTile(new Vector3Int(tilePos.x, tilePos.y + 1));
                UpdateSideTile(new Vector3Int(tilePos.x, tilePos.y - 1));
            }
        }


        hits.Clear();
    }


    void UpdateSideTile(Vector3Int _tilePos)
    {
        bool haveTileUp;
        if (tilemap.GetTile(new Vector3Int(_tilePos.x + 1, _tilePos.y) ) != null)
        {
            haveTileUp = true;
        }
        else
        {
            haveTileUp = false;
        }

        bool haveTileDown;
        if (tilemap.GetTile(new Vector3Int(_tilePos.x - 1, _tilePos.y)) != null)
        {
            haveTileDown = true;
        }
        else
        {
            haveTileDown = false;
        }
        bool haveTileRight;
        if (tilemap.GetTile(new Vector3Int(_tilePos.x, _tilePos.y + 1)) != null)
        {
            haveTileRight = true;
        }
        else
        {
            haveTileRight = false;
        }
        bool haveTileLeft;
        if (tilemap.GetTile(new Vector3Int(_tilePos.x, _tilePos.y - 1)) != null)
        {
            haveTileLeft = true;
        }
        else
        {
            haveTileLeft = false;
        }



        if (haveTileUp && haveTileDown && haveTileRight && haveTileLeft)
        {
            //LOS TIENE TODOS
        }
        else if (!haveTileUp && haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE ARRIBA
        }
        else if (haveTileUp && !haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE ABAJO

        }
        else if (haveTileUp && haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE LA DERECHA

        }
        else if (haveTileUp && haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTA EL DE LA IZQUIERDA

        }
        else if (!haveTileUp && !haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y ABAJO

        }
        else if (haveTileUp && haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE IZQUIERDA Y DERECHA
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {
            float range = drillDistance - (i / 90); //Esta resta se hace para que los rayos no esten todos a la misma distancia como su fuera un circulo
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * range);

            direction = transform.rotation * Quaternion.Euler(0, 0, -i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * range);
        }
    }
}

