using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class DrillController : MonoBehaviour
{
    //[SerializeField]
    //private Tilemap tilemap;
    //[SerializeField]
    //private Grid grid;

    [SerializeField]
    private float drillDistance;
    [SerializeField]
    private float maxDrillAngle;
    [SerializeField]
    private int raysPerSide;
    
    [SerializeField]
    private LayerMask breakableWallLayer;


    [Space, SerializeField]
    private TileBase baseTile;
    [SerializeField]
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

    private BreakableWallController breakableWallController;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Drill();        
    }

    private void Drill()
    {

        List<RaycastHit2D> hits = CheckSomethingToDrill();

        DrillWalls(hits);

        hits.Clear();
    }

    private List<RaycastHit2D> CheckSomethingToDrill()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
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

        return hits;
    }
    private void DrillWalls(List<RaycastHit2D> _hits)
    {
        

        int totalHits = 0;
        foreach (RaycastHit2D hit in _hits)
        {
            if (hit && hit.rigidbody.CompareTag("BreakableWall"))
            {
                if (!breakableWallController)
                {
                    breakableWallController = hit.rigidbody.GetComponent<BreakableWallController>();
                    if (breakableWallController.isHide)
                    {
                        breakableWallController = null;
                        return;
                    }
                }

                breakableWallController.ChangeTileContent(hit.centroid, null);

                Vector2 gridOffset = breakableWallController.GetGridOffset();
                Vector2 tilePos = hit.centroid;
                UpdateSideTile(new Vector2(tilePos.x + gridOffset.x, tilePos.y));
                UpdateSideTile(new Vector2(tilePos.x - gridOffset.x, tilePos.y));
                UpdateSideTile(new Vector2(tilePos.x, tilePos.y + gridOffset.y));
                UpdateSideTile(new Vector2(tilePos.x, tilePos.y - gridOffset.y));

                totalHits++;
            }
        }

        if (totalHits == 0)
        {
            breakableWallController = null;
        }
    }

    void UpdateSideTile(Vector2 _tilePos)
    {
        Vector2 gridOffset = breakableWallController.GetGridOffset();

        if (breakableWallController.GetTileContent(_tilePos) == null)
            return;

        bool haveTileUp;
        if (breakableWallController.GetTileContent(new Vector3(_tilePos.x, _tilePos.y - gridOffset.y)) != null)
        {
            haveTileUp = true;
        }
        else
        {
            haveTileUp = false;
        }

        bool haveTileDown;
        if (breakableWallController.GetTileContent(new Vector3(_tilePos.x, _tilePos.y + gridOffset.y)) != null)
        {
            haveTileDown = true;
        }
        else
        {
            haveTileDown = false;
        }
        bool haveTileRight;
        if (breakableWallController.GetTileContent(new Vector3(_tilePos.x - gridOffset.x, _tilePos.y )) != null)
        {
            haveTileRight = true;
        }
        else
        {
            haveTileRight = false;
        }
        bool haveTileLeft;
        if (breakableWallController.GetTileContent(new Vector3(_tilePos.x + gridOffset.x, _tilePos.y)) != null)
        {
            haveTileLeft = true;
        }
        else
        {
            haveTileLeft = false;
        }

        TileBase currentTile = null;

        if (haveTileUp && haveTileDown && haveTileRight && haveTileLeft)
        {
            //LOS TIENE TODOS
            currentTile = baseTile;
        }
        else if (!haveTileUp && haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE ARRIBA
            currentTile = threeSideTileDown;
        }
        else if (haveTileUp && !haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE ABAJO
            currentTile = threeSideTileUp;
        }
        else if (haveTileUp && haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTA EL DE LA DERECHA
            currentTile = threeSideTileLeft;
        }
        else if (haveTileUp && haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTA EL DE LA IZQUIERDA
            currentTile = threeSideTileRight;
        }
        else if (!haveTileUp && !haveTileDown && haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y ABAJO
            currentTile = twoSideTileRightLeft;
        }
        else if (haveTileUp && haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE IZQUIERDA Y DERECHA
            currentTile = twoSideTileUpDown;
        }
        else if (!haveTileUp && haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y DERECHA
            currentTile = twoSideTileUpRight;

        }
        else if (!haveTileUp && haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA Y IZQUIERDA
            currentTile = twoSideTileUpLeft;

        }
        else if (haveTileUp && !haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ABAJO Y DERECHA
            currentTile = twoSideTileDownRight;

        }
        else if (haveTileUp && !haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ABAJO Y IZQUIERDA
            currentTile = twoSideTileDownLeft;
        }
        else if (haveTileUp && !haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ABAJO, DERECHA Y IZQUIERDA
            currentTile = oneSideTileDown;
        }
        else if (!haveTileUp && haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA, DERECHA Y IZQUIERDA
            currentTile = oneSideTileUp;
        }
        else if (!haveTileUp && !haveTileDown && haveTileRight && !haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA, ABAJO Y IZQUIERDA
            currentTile = oneSideTileLeft;
        }
        else if (!haveTileUp && !haveTileDown && !haveTileRight && haveTileLeft)
        {
            //LE FALTAN LOS DE ARRIBA, ABAJO Y DERECHA
            currentTile = oneSideTileRight;
        }
        else if (!haveTileUp && !haveTileDown && !haveTileRight && !haveTileLeft)
        {
            //LE FALTAN TODOS
            currentTile = aloneTile;
        }

        breakableWallController.ChangeTileContent(_tilePos, currentTile);

    }


    private void OnDrawGizmosSelected()
    {
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {
            Gizmos.color = Color.magenta;
            float range = drillDistance - (i / 90); //Esta resta se hace para que los rayos no esten todos a la misma distancia como su fuera un circulo
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * range);

            Gizmos.color = Color.magenta;
            direction = transform.rotation * Quaternion.Euler(0, 0, -i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * range);

        }

    }

    
}

