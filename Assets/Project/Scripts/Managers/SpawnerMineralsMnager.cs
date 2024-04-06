using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SpawnerMineralsMnager : MonoBehaviour
{
    public int lastSpawner;

    public static SpawnerMineralsMnager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        lastSpawner = -1; 
    }
}
