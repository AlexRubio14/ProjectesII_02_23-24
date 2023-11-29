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
    // Update is called once per frame
    void Update()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            hits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.up).normalized, drillDistance, breakableWallLayer));
            direction = transform.rotation * Quaternion.Euler(0, 0, -i);
            hits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.up).normalized, drillDistance, breakableWallLayer));
        }

        hits.ForEach(hit =>
        {
            if (hit)
            {
                Vector3Int tilePos = grid.LocalToCell(hit.centroid);
                if (tilemap.GetTile(tilePos) == null)
                {
                    if (tilePos.x - hit.centroid.x > 0.7f 
                    || tilePos.x - hit.centroid.x < -0.7f)
                    {
                        tilePos.x--;
                    }

                    if (tilePos.y - hit.centroid.y > 0.7f
                    || tilePos.y - hit.centroid.y < -0.7f)
                    {
                        tilePos.y--;
                    }

                }

                tilemap.SetTile(tilePos, null);
            }
        });

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (float i = 0; i < maxDrillAngle; i += maxDrillAngle / raysPerSide)
        {

            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * drillDistance);

            direction = transform.rotation * Quaternion.Euler(0, 0, -i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.up) * drillDistance);
        }
    }
}

