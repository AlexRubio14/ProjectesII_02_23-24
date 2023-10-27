using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float radius = 2.0f;
    [SerializeField]
    private float agentColliderSize = 0.6f;

    // DEBUG 
    [SerializeField]
    private bool showGizmos = true;
    float[] dangersResultTemp = null; 

    
    // calculate the danger obstacles
    public override (float[] danger, float[] interest) GetSteering(float[] _danger, float[] _interest, IAData _iaData) // Anonymous Type
    {
        // Look all the obstacles (not player)
        foreach(Collider2D obstacleCollider in _iaData.m_obstacles)
        {
            Vector2 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float distanceToObstacle = directionToObstacle.magnitude; 

            // Calculate weight based on the distance Enemy<--->Obstacle
            float weight = distanceToObstacle <= agentColliderSize ? 
                1 : (radius - distanceToObstacle) / radius; // if... very near = 1(not good)

            Vector2 directionToObstacleNormalized = directionToObstacle.normalized; 

            // Add obstacle parameters to the danger array
            for(int i = 0; i < Directions.m_eightDirections.Count; i++)
            {
                float result = Vector2.Dot(directionToObstacleNormalized, Directions.m_eightDirections[i]); // me da valores de 1, -1 o 0

                float valueToPutIn = result * weight; 

                // Override value only if it's higher than the current one stored in the danger array
                if(valueToPutIn > _danger[i])
                {
                    _danger[i] = valueToPutIn;
                }
            }
        }

        //DEBUG
        dangersResultTemp = _danger;


        return (_danger, _interest); // interest not modified
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return; 

        if(Application.isPlaying && dangersResultTemp != null)
        {
            if(dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for(int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.m_eightDirections[i] * dangersResultTemp[i]);
                }
            }
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius); 
        }
    }
}