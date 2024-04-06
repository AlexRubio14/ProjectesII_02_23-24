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

    [SerializeField]
    private AudioClip spottedPlayerClip;


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
        switch (currentEnemy.currentState)
        {
            case EnemyStates.PATROLLING:
                _iaData.canSeeTarget = true;
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
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rangeVision, collidersLayer);

            // Check if the collider we see is on the "Player" layer
            if (hit.collider != null && (playerLayer & (1 << hit.collider.gameObject.layer)) != 0
                /* detect that we found a player not an obstacle*/)
            {
                currentEnemy.ChangeState(EnemyStates.CHASING);

                _iaData.canSeeTarget = true;
                _iaData.m_currentTarget = playerCollider.transform;
                AudioManager.instance.Play2dOneShotSound(spottedPlayerClip, "Enemy");
            }
        }
    }

    private void CheckIfSeeTarget(IAData _iaData)
    {
        // Check if enemy sees the player
        if (_iaData.m_currentTarget == null)
            return; 

        Vector2 direction = (_iaData.m_currentTarget.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rangeVision, collidersLayer);

        // Check if the collider we see is on the "Player" layer
        if (hit.collider != null && (playerLayer & (1 << hit.collider.gameObject.layer)) != 0
            /* detect that we found a player not an obstacle*/)
        {
            _iaData.canSeeTarget = true;
            Debug.DrawRay(transform.position, direction * rangeVision, Color.magenta);
        }
        else
        {
            _iaData.canSeeTarget = false;
        }
    }

    //DEBUG
    private void OnDrawGizmosSelected()
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
