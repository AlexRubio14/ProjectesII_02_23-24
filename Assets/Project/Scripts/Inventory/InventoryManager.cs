using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField, SerializedDictionary("Item", "Amount")]
    private SerializedDictionary<ItemObject, short> allItems;
    private Dictionary<ItemObject, short> runItems;
    public Action<ItemObject, short> obtainItemAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);


        LoadItems();

        runItems = new Dictionary<ItemObject, short>();
    }

    private void LoadItems()
    {
        Dictionary<ItemObject, short> storedItems = new Dictionary<ItemObject, short>();
        foreach (KeyValuePair<ItemObject, short> item in allItems)
        {
            if (PlayerPrefs.HasKey(item.Key.ItemName))
            {
                storedItems.Add(item.Key, (short)PlayerPrefs.GetInt(item.Key.ItemName));
            }
        }

        foreach (KeyValuePair<ItemObject, short> item in storedItems)
        {
            allItems[item.Key] = item.Value;
        }
    }
    public void SaveItems()
    {
        foreach (KeyValuePair<ItemObject, short> item in allItems)
        {
            PlayerPrefs.SetInt(item.Key.ItemName, item.Value);
        }
    }

    public void ResetInventory()
    {
        SerializedDictionary<ItemObject, short> storedItems = new SerializedDictionary<ItemObject, short>();

        foreach (KeyValuePair<ItemObject, short> item in allItems)
        {
            storedItems.Add(item.Key, 0);
        }

        allItems = storedItems;

        SaveItems();

    }

    public void ChangeRunItemAmount(ItemObject _itemType, short _itemsToAdd)
    {
        if (!runItems.ContainsKey(_itemType))
        {
            runItems.Add(_itemType, _itemsToAdd);
        }
        else
        {
            runItems[_itemType] += _itemsToAdd;
            if (runItems[_itemType] < 0)
                runItems[_itemType] = 0;
        }

        if(obtainItemAction != null)
            obtainItemAction(_itemType, _itemsToAdd);
    }

    public bool CanBuy(Dictionary<ItemObject, short> _upgradePrize)
    {
        foreach (KeyValuePair<ItemObject, short> item in _upgradePrize)
        {
            if (allItems[item.Key] - item.Value < 0)
            {
                return false;
            }
        }
        return true;
    }

    /* ANTES DE LLAMAR A ESTA FUNCION SE HA DE CALCULAR QUE SE TENGAN LOS MATERIALES NECESARIOS */
    public void Buy(Dictionary<ItemObject, short> _upgradePrize)
    {
        foreach (KeyValuePair<ItemObject, short> item in _upgradePrize)
        {
            allItems[item.Key] -= item.Value;
        }

        SaveItems();
    }


    public Dictionary<ItemObject, short> GetSavetems()
    {
        return allItems;
    }
    public Dictionary<ItemObject, short> GetRunItems()
    {
        return runItems;
    }
    public Dictionary<ItemObject, short> GetAllItems()
    {
        Dictionary<ItemObject, short> currentAllItems = new Dictionary<ItemObject, short>();

        foreach (KeyValuePair<ItemObject, short> item in allItems)
        {
            short itemsToAdd = runItems.ContainsKey(item.Key)? (short)(item.Value + runItems[item.Key]) : item.Value;
            currentAllItems.Add(item.Key, itemsToAdd);
        }


        return currentAllItems;
    }
    public short GetTotalItemAmount(ItemObject _item)
    {
        short totalItemAmount = 0;

        totalItemAmount += allItems[_item];
        if (runItems.ContainsKey(_item))
            totalItemAmount += runItems[_item];

        return totalItemAmount;
    }

    public void EndRun(bool _alive)
    {
        if (_alive)
        {
            foreach (KeyValuePair<ItemObject, short> item in runItems)
            {
                if (item.Key.PowerUp == PowerUpManager.PowerUpType.NONE )
                {
                    allItems[item.Key] += item.Value;
                }
                else
                {
                    PowerUpManager.Instance.PowerUpObtained(item.Key.PowerUp);
                }
            }

        }

        runItems.Clear();

        SaveItems();
    }
}