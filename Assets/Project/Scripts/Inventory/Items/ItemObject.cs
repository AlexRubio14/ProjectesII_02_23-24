using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemObject : ScriptableObject
{

    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField]
    public Sprite BetaSprite { get; private set; }

    [field: SerializeField]
    public Sprite MapSprite { get; private set; }

    [field: SerializeField]
    public Sprite PickableSprite { get; private set; }

    [field: SerializeField]
    public Color EffectsColor { get; private set; }

    [field: SerializeField]
    public float BaseMineralHealth {  get; private set; }
    [field: SerializeField]
    public Vector2 EnergyLevelSize { get; private set; }

    [field: Tooltip("En caso de no tener power up, dejarlo en NONE"), SerializeField]
    public PowerUpManager.PowerUpType PowerUp { get; private set; }

}
