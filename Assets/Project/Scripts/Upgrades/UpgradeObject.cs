using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]

public class UpgradeObject : ScriptableObject
{
    [field: SerializeField]
    public string UpgradeName { get; private set; }

    [field: SerializeField, TextArea]
    public string UpgradeDescription { get; private set; }

    [field: SerializeField]
    public Sprite c_UpgradeSprite { get; private set; }

    [SerializeField]
    private List<ItemObject> itemToUpgrade;
    [SerializeField]
    private List<short> itemPrize;

    public Dictionary<ItemObject, short> prize { get; private set; }

    public void Awake()
    {
        prize = new Dictionary<ItemObject, short>();
        for (int i = 0; i < itemToUpgrade.Count; i++)
        {
            prize.Add(itemToUpgrade[i], itemPrize[i]);
        }
    }

}
