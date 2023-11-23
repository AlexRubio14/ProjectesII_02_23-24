using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField, AYellowpaper.SerializedCollections.SerializedDictionary("Item", "Amount")]
    private AYellowpaper.SerializedCollections.SerializedDictionary<ItemObject, short> allItems;
    private Dictionary<ItemObject, short> runItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        runItems = new Dictionary<ItemObject, short>();
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

        EventManager.CallOnItemChange(_itemType, _itemsToAdd);
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
    }


    public Dictionary<ItemObject, short> GetAllItems()
    {
        return allItems;
    }
    public Dictionary<ItemObject, short> GetRunItems()
    {
        return runItems;
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
    }
   
}