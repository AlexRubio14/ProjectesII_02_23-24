using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public Dictionary<UpgradeObject, bool> UpgradeObtained;

    [SerializeField]
    private UpgradeObject[] allUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        Instance = this;
        DontDestroyOnLoad(Instance);

        UpgradeObtained = new Dictionary<UpgradeObject, bool>();

        foreach (UpgradeObject item in allUpgrades)
        {
            UpgradeObtained.Add(item, false);
        }
    }

    public void ObtainUpgrade(UpgradeObject _currentUpgrade)
    {
        UpgradeObtained[_currentUpgrade] = true;
    }






}
