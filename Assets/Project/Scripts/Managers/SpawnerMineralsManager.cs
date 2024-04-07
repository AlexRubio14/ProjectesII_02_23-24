using UnityEngine;

public class SpawnerMineralsManager : MonoBehaviour
{
    public int lastSpawner;

    public static SpawnerMineralsManager Instance;
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
