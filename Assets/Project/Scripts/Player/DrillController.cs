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

    private void Start()
    {
        hits = new List<RaycastHit2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {
            float range = drillDistance - (i / 70); //Esta resta se hace para que los rayos no esten todos a la misma distancia como su fuera un circulo
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
            }
        }


        hits.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {
            float range = drillDistance - (i / 70); //Esta resta se hace para que los rayos no esten todos a la misma distancia como su fuera un circulo
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * range);

            direction = transform.rotation * Quaternion.Euler(0, 0, -i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * range);
        }
    }
}

