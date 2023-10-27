using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public enum ItemType { Cooper, Emerald, Enemy1, Enemy2, Plant };


    [field: SerializeField]
    public ItemType type { get; private set; }

    [field: SerializeField]
    public float weight { get; private set; }
}
