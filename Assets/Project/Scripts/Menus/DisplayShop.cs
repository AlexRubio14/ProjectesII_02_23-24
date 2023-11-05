using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class DisplayShop : MonoBehaviour
{
    [SerializeField]
    private Image[] images;

    [SerializeField]
    private TextMeshProUGUI[] texts;

    void Start()
    {
        UpdateInventory(); 
    }

    private void UpdateInventory()
    {
        Dictionary<ItemObject, short> inventoryMap = InventoryManager.Instance.GetAllItems();
        int count = 0; 

        foreach (KeyValuePair<ItemObject, short> item in inventoryMap)
        {
            texts[count].text = "x" + item.Value;
            images[count].sprite = item.Key.c_pickableSprite;
            count++; 
        }
    }

    public void BuyUpgrade(UpgradeObject _upgradeObject)
    {
        if (InventoryManager.Instance.CanBuy(_upgradeObject.prize))
        {
            InventoryManager.Instance.BuyUpgrade(_upgradeObject.prize); 
            // UpgradeManager.Instance.
        }
    }
}
