using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void OnItemChange(ItemController.ItemType _itemType, short _itemAmount);
    public static event OnItemChange onItemChange;
    public static void CallOnItemChange(ItemController.ItemType _itemType, short _itemAmount)
    {
        if (onItemChange != null)
        {
            onItemChange(_itemType, _itemAmount);
        }
    }
}
