using System.Collections.Generic;
using UnityEngine;

public class SelectTpsManager : MonoBehaviour
{
    public static SelectTpsManager instance;

    [field: SerializeField]
    public List<TpObject> tpList { get; private set; }

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

        LoadTps();

    }

    public void AddDiscoveredTp(int key)
    {
        for(int i = 0; i < tpList.Count; i++)
        {
            if (tpList[i].id == key)
            {
                tpList[i].discovered = true;
                SaveTPs();
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

    public void ResetTpObjects()
    {
        for (int i = 0; i < tpList.Count; i++)
        {
            tpList[i].discovered = false;
        }

        SaveTPs();
    }

    private void LoadTps()
    {
        foreach (TpObject item in tpList)
            item.discovered = PlayerPrefs.HasKey("Tp_" + item.id) && PlayerPrefs.GetInt("Tp_" + item.id) == 1;
    }

    public void SaveTPs()
    {
        foreach (TpObject item in tpList)
            PlayerPrefs.SetInt("Tp_" + item.id, item.discovered ? 1 : 0);

        PlayerPrefs.Save();
    }

}
