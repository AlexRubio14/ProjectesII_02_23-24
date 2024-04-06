using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerMineralsMnager : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] spawners;

    private int lastSpawner; 

    void Start()
    {
        int currentSpawner = Random.Range(0, spawners.Length);

        for(int i = 0; i < spawners.Length; i++)
        {
            if(i == currentSpawner)
                spawners[i].SetActive(true);
            else
                spawners[i].SetActive(false);
        }
    }
}
