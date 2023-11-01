using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemObject : ScriptableObject
{

    [field: SerializeField]
    public string itemName { get; private set; }

    [field: SerializeField]
    public float weight { get; private set; }

    [field: SerializeField]
    public Sprite c_sprite { get; private set; }

    [field: SerializeField]
    public Color lightColor;

}
