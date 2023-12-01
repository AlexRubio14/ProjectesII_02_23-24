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

    [AYellowpaper.SerializedCollections.SerializedDictionary("Item", "Amount")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<ItemObject, short> prize;

}
