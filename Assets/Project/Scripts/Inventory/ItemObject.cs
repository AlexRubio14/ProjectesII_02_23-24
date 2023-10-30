using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemObject : ScriptableObject
{
    public enum ItemType { Cooper, Emerald, Enemy1, Enemy2, Plant };


    [field: SerializeField]
    public ItemType type { get; private set; }


    [field: SerializeField]
    public string itemName { get; private set; }

    [field: SerializeField]
    public float weight { get; private set; }

    [field: SerializeField]
    public Sprite c_sprite { get; private set; }



}
