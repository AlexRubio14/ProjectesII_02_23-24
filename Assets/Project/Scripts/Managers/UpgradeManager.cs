using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private Dictionary<UpgradeObject, bool> UpgradesObtained;

    [SerializeField]
    private UpgradeObject[] allUpgrades;

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

        UpgradesObtained = new Dictionary<UpgradeObject, bool>();

        foreach (UpgradeObject item in allUpgrades)
        {
            UpgradesObtained.Add(item, false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            foreach (UpgradeObject item in allUpgrades)
            {
                UpgradesObtained[item] = true;
            }
        }
    }

    public void ObtainUpgrade(UpgradeObject _currentUpgrade)
    {
        UpgradesObtained[_currentUpgrade] = true;
    }

    public bool CheckObtainedUpgrade(UpgradeObject _currentUpgrade)
    {
        if (UpgradesObtained.ContainsKey(_currentUpgrade))
            return UpgradesObtained[_currentUpgrade];
        else
            return false;
    }


}
