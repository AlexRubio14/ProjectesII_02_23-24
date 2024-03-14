using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTpsManager : MonoBehaviour
{
    public static SelectTpsManager _instance;

    private Dictionary<int, Transform> discoveredTpsPositions = new Dictionary<int, Transform>();

    private int totalTpsId = 1;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void AddDiscoveredTp(int key, Transform tpPosition)
    {
        discoveredTpsPositions.Add(key, tpPosition);
    }

    public Transform SearchTp(int tpId)
    {
        foreach(int key in discoveredTpsPositions.Keys)
        {
            if(key == tpId)
            {
                return discoveredTpsPositions[key];
            }
        }
        return null;
    }

    public int GetTotalIds()
    {
        return totalTpsId;
    }
    public void AddTotalIds(int value)
    {
        totalTpsId++;
    }
}
