using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class TargetDetector : Detector
{
    private Enemy currentEnemy;

    [SerializeField]
    private float rangeVision = 5.0f;

    [SerializeField]
    private LayerMask playerLayer, collidersLayer;

    // DEBUG
    [SerializeField]
    private bool showGizmos = false;
    private List<Transform> colliders;

    private void Awake()
    {
        currentEnemy = GetComponentInParent<Enemy>();
    }

    public override void Detect(IAData _iaData)
    {
        switch (currentEnemy.GetState())
        {
            case EnemyStates.PATROLLING:
                CheckIfPlayerArround(_iaData); 
                break;
            case EnemyStates.CHASING:
                CheckIfSeeTarget(_iaData);
                break;
            default:
                CheckIfSeeTarget(_iaData);
                break;
        }
    }

    private void CheckIfPlayerArround(IAData _iaData)
    {
        // Find out if player is near
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, rangeVision, playerLayer);

        if (playerCollider != null)
        {
            currentEnemy.currentState = Enemy.EnemyStates.CHASING;

            _iaData.m_currentTarget = playerCollider.transform;
        }
        //else
        //{
        //    colliders = null;
        //}

        //_iaData.m_targets = colliders; 
    }

    private void CheckIfSeeTarget(IAData _iaData)
    {
        // Check if enemy sees the player
        Vector2 direction = (_iaData.m_currentTarget.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rangeVision, collidersLayer);

        // Check if the collider we see is on the "Player" layer
        if (hit.collider != null && (playerLayer & (1 << hit.collider.gameObject.layer)) != 0
            /* detect that we found a player not an obstacle*/)
        {
            _iaData.canSeeTarget = true;
            //colliders = new List<Transform>() { playerCollider.transform };
            Debug.DrawRay(transform.position, direction * rangeVision, Color.magenta);
        }
        else
        {
            _iaData.canSeeTarget = false;
            //_iaData.m_currentTarget = null; 
            //colliders = null;
        }
        //_iaData.m_targets = colliders;
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
