using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserManager : MonoBehaviour
{
    [SerializeField]
    private int numberOfBalls = 20;

    private List<GameObject> l_lasers;

    [SerializeField]
    private GameObject laser;

    private void Awake()
    {
        for(int i = 0; i < numberOfBalls; i++)
        {
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShootLaser()
    {

    }
}
