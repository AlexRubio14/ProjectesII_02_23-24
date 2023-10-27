using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    // DEBUG
    [SerializeField]
    private bool showGizmos = true;
    private float[] interestGizmo = new float[0]; 
    private Vector2 resultDirection = Vector2.zero;
    private float rayLenght = 1;

    private void Start()
    {
        interestGizmo = new float[8];
    }

    public Vector2 GetDirectionToMove(List<SteeringBehaviour> _behaviours, IAData _iaData)
    {
        float[] danger = new float[8]; 
        float[] interest = new float[8];

        // Loop through each behaviour
        foreach(SteeringBehaviour behaviour in _behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, _iaData); // relleno danger and interest
        }

        // Subtract danger values from interest array: removes the direction that we don't want to use for movement
        for(int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]); // return 0 if the number is negative
        }
        //DEBUG
        interestGizmo = interest;

        // Get the average direction
        Vector2 outputDirection = Vector2.zero; 
        for(int i = 0; i < 8; i++)
        {
            outputDirection += Directions.m_eightDirections[i] * interest[i]; 
        }
        outputDirection.Normalize();

        //DEBUG
        resultDirection = outputDirection;

        // return the selected movement direction
        return resultDirection; 
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLenght); 
        }
    }
}
