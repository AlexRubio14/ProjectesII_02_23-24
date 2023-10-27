using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private Dictionary<ItemController.ItemType, short> items;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
        DontDestroyOnLoad(Instance);

        items = new Dictionary<ItemController.ItemType, short>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeItemtAmount((ItemController.ItemType)Random.Range(0, 5), 1);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeItemtAmount((ItemController.ItemType)Random.Range(0, 5), -1);
        }
    }

    public void ChangeItemtAmount(ItemController.ItemType _itemType, short _itemsToAdd)
    {
        if (!items.ContainsKey(_itemType))
        {
            items.Add(_itemType, _itemsToAdd);
        }
        else
        {
            items[_itemType] += _itemsToAdd;
            if (items[_itemType] < 0)
                items[_itemType] = 0;
        }

        EventManager.CallOnItemChange(_itemType, _itemsToAdd);
    }

    public Dictionary<ItemController.ItemType, short> GetItems()
    {
        return items;
    }

    /* 
     * ANTES DE LLAMAR A ESTA FUNCION SE HA DE CALCULAR QUE SE TENGAN LOS MATERIALES NECESARIOS
     */
    public void BuyItem(Dictionary<ItemController.ItemType, short> _boughtItems)
    {
        foreach (KeyValuePair<ItemController.ItemType, short> item in _boughtItems)
        {
            items[item.Key] -= item.Value;
        }
    }   
}