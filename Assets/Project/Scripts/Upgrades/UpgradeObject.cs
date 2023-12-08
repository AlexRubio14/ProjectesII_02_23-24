using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]

public class UpgradeObject : ScriptableObject
{
    public enum UpgradeType { BOOST, LIGHT, DRILL, CORE_COLLECTOR }

    [field : SerializeField]
    public UpgradeType type { get; private set; }
    [field : SerializeField]
    public string UpgradeName { get; private set; }

    [field : SerializeField, TextArea]
    public string UpgradeDescription { get; private set; }

    [field : SerializeField]
    public Sprite c_UpgradeSprite { get; private set; }

    [SerializedDictionary("Item", "Amount")]
    public SerializedDictionary<ItemObject, short> prize;

}
