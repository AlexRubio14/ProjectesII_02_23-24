using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyIA : MonoBehaviour
{
    [SerializeField]
    protected List<SteeringBehaviour> l_steeringBehaviours;

    [SerializeField]
    private List<Detector> l_detectors;

    [SerializeField]
    protected IAData iaData;

    [SerializeField]
    private float detectionDelay = 0.05f;

    [SerializeField]
    protected ContextSolver movementDirectionSolver;

    public bool isFollowing { get; protected set; }

    [SerializeField]
    protected float speed = 5.0f; 

    protected Rigidbody2D c_rb2d; 

    public void InitEnemy()
    {
        c_rb2d = GetComponent<Rigidbody2D>();

        isFollowing = false; 

        // Detecting player and obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay); 
    }

    private void PerformDetection()
    {
        // DETECTORS
        foreach (Detector detector in l_detectors)
        {
            detector.Detect(iaData);
        }
    }

    public void CheckIsFollowing()
    {
        // Enemy AI movement based on target availability 
        if (iaData.m_currentTarget != null)
        {
            // Looking at the target
            if (isFollowing == false)
            {
                isFollowing = true;
            }
        }
        else if (iaData.m_currentTarget == null && iaData.GetTargetsCount() > 0) // pick a target if you don't have one
        {
            // Target acquisition logic
            iaData.m_currentTarget = iaData.m_targets[0];
        }
        else
        {
            isFollowing = false;
            Debug.Log("Stopping");
        }
    }

    public void Movement()
    {
        // Enemy AI movement based on target availability 
        if(iaData.m_currentTarget != null)
        {
            // Looking at the target
            if(isFollowing == false)
            {
                isFollowing = true;
            }
        }
        else if(iaData.m_currentTarget == null && iaData.GetTargetsCount() > 0) // pick a target if you don't have one
        {
            // Target acquisition logic
            iaData.m_currentTarget = iaData.m_targets[0]; 
        }
        else
        {
            isFollowing = false;
            Debug.Log("Stopping"); 
        }

        if(isFollowing)
        {
            Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);

            c_rb2d.AddForce(direction * speed, ForceMode2D.Force);

            // ROTATION OF THE ENENMY WHILE FOLLOWING
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
    }
}
