using UnityEngine;
using AYellowpaper.SerializedCollections;
[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]

public class UpgradeObject : ScriptableObject
{
    public enum UpgradeType { BOOST, LIGHT, DRILL, SIZE_CHANGER, CORE_COLLECTOR }

    [field : SerializeField]
    public UpgradeType type { get; private set; }
    [field : SerializeField]
    public string UpgradeName { get; private set; }

    [field : SerializeField, TextArea]
    public string UpgradeDescription { get; private set; }

    [field : SerializeField]
    public Sprite UpgradeSprite { get; private set; }

}
