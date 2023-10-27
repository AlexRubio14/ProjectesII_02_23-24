using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField]
    private float detectionRange = 2.0f;

    [SerializeField]
    private LayerMask layerToAvoid;

    //DEBUG
    [SerializeField]
    private bool showGizmos = true; 
    Collider2D[] colliders; 

    public override void Detect(IAData _iaData)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, layerToAvoid);
        _iaData.m_obstacles = colliders;
    }

    //DEBUG
    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        if(Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach(Collider2D collider in colliders)
            {
                Gizmos.DrawSphere(collider.transform.position, 0.3f); 
            }
        }
    }

}
