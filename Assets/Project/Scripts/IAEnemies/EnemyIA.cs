using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyIA : MonoBehaviour
{
    [Header("--- IA"), SerializeField]
    protected List<SteeringBehaviour> steeringBehaviours;

    [SerializeField] 
    protected List<Detector> detectors;

    [SerializeField]
    protected IAData iaData;

    //[SerializeField]
    //private float detectionDelay = 0.05f;

    [SerializeField]
    protected ContextSolver movementDirectionSolver;

    protected void PerformDetection()
    {
        foreach (Detector detector in detectors)
            detector.Detect(iaData);
    }
}
