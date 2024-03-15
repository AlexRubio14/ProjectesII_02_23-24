using System.Collections.Generic;
using UnityEngine;

public class SelectTpsManager : MonoBehaviour
{
    public static SelectTpsManager instance;

    [SerializeField]
    private List<TpObject> tpList;

    [SerializeField]
    private int idToTeleport;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void AddDiscoveredTp(int key)
    {
        for(int i = 0; i < tpList.Count; i++)
        {
            if (tpList[i].id == key)
            {
                tpList[i].discovered = true;
                return;
            }
        }
    }

    public int GetIdToTeleport()
    {
        return idToTeleport;
    }

    public void SetIdToTeleport(int value)
    {
        idToTeleport = value;
    }

    public int GetSizeOfTpList()
    {
        return tpList.Count;
    }
}
