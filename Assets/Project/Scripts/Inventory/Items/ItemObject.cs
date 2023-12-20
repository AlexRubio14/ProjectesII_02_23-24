using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemObject : ScriptableObject
{

    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField]
    public float Weight { get; private set; }

    [field: SerializeField]
    public Sprite c_MapSprite { get; private set; }

    [field: SerializeField]
    public Sprite c_PickableSprite { get; private set; }

    [field: SerializeField]
    public Color EffectsColor { get; private set; }
    


    [field: Tooltip("En caso de no tener power up, dejarlo en NONE"), SerializeField]
    public PowerUpManager.PowerUpType PowerUp { get; private set; }

}
