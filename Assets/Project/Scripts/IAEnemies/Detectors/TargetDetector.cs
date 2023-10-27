using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField]
    private float rangeVision = 5.0f;

    [SerializeField]
    private LayerMask playerLayer, collidersLayer;

    // DEBUG
    [SerializeField]
    private bool showGizmos = false;
    private List<Transform> colliders; 

    public override void Detect(IAData _iaData)
    {
        // Find out if player is near
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, rangeVision, playerLayer);

        if (playerCollider != null)
        {
            // Check if enemy sees the player
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rangeVision, collidersLayer);

            // Check if the collider we see is on the "Player" layer
            if (hit.collider != null && (playerLayer & (1 << hit.collider.gameObject.layer)) != 0
                /* detect that we found a player not an obstacle*/)
            {
                colliders = new List<Transform>() { playerCollider.transform };
                Debug.DrawRay(transform.position, direction * rangeVision, Color.magenta);
            }
            else
            {
                colliders = null;
            }
        }
        else
        {
            colliders = null;
        }

        _iaData.m_targets = colliders; 
    }

    //DEBUG
    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return; 

        Gizmos.DrawWireSphere(transform.position, rangeVision);

        if (colliders == null)
            return; 

        Gizmos.color = Color.magenta;
        foreach(var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }

}
