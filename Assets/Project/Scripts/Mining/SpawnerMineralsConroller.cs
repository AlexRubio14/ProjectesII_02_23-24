using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerMineralsConroller : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawners;

    void Awake()
    {
        int currentSpawner;
        int lastSpawner = SpawnerMineralsManager.Instance.lastSpawner; 
        do
        {
            currentSpawner = Random.Range(0, spawners.Length);
        }
        while (lastSpawner == currentSpawner);


        spawners[currentSpawner].SetActive(true);

        SpawnerMineralsManager.Instance.lastSpawner = currentSpawner;
    }
}
