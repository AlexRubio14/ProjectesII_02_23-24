using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float targetReachedThreshold = 0.5f; // target alcanzado

    private bool reachedLastTarget = true;

    private Vector2 targetPositionCached; // player position

    //DEBUG
    [SerializeField]
    private bool showGizmos = true;
    private float[] interestTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] _danger, float[] _interest, IAData _iaData)
    {
        // if... we don't have a target stop seeking
        if (reachedLastTarget)
        {
            if(_iaData.m_currentTarget == null)
                return (_danger, _interest);
        }

        // Cache the last position only if we still see the target (if the targets collection is not empty)
        if (_iaData.m_currentTarget != null && _iaData.canSeeTarget)
        {
            targetPositionCached = _iaData.m_currentTarget.position;
        }

        // if we have reached the target
        if (Vector2.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            _iaData.m_currentTarget = null;
            return (_danger, _interest); // Nothing to chase because we have already chase it 
        }

        // if we haven't yet reached the target do the main logic of finding the interest directions
        Vector2 directionToTarget = targetPositionCached - (Vector2)transform.position;
        for (int i = 0; i < _interest.Length; i++)
        {
            // Closes direction
            float result = Vector2.Dot(directionToTarget.normalized, Directions.m_eightDirections[i]);

            // Accept only directions at the less than 90º to the target direction
            if (result > 0)
            {
                float valueToPutIn = result;
                // Override value only if it's higher than the current one stored in the danger array
                if (valueToPutIn > _interest[i])
                {
                    _interest[i] = valueToPutIn;
                }
            }
        }

        // DEBUG
        interestTemp = _interest;


        return (_danger, _interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        Gizmos.DrawSphere(targetPositionCached, 0.2f); 

        if(Application.isPlaying && interestTemp != null)
        {
            if (interestTemp != null)
            {
                Gizmos.color = Color.green; 
                for(int i = 0; i < interestTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.m_eightDirections[i] * interestTemp[i]);
                }
                if(reachedLastTarget == false)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(targetPositionCached, 0.1f); 
                }
            }
        }
    }

}
