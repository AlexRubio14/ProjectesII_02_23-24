using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject menuItemPrefab;
    Dictionary<ItemObject, Image> itemsImages; 
    private void Start()
    {
        itemsImages = new Dictionary<ItemObject, Image>();
        SetupItemList();
    }


    private void SetupItemList()
    {
        Dictionary<ItemObject, short> inventory = InventoryManager.Instance.GetAllItems();

        foreach (KeyValuePair<ItemObject, short> item in inventory)
        {
            Image newItem = Instantiate(menuItemPrefab, transform).GetComponent<Image>();
            newItem.sprite = item.Key.c_PickableSprite;
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.ToString();

            itemsImages.Add(item.Key, newItem);
        }
    }


    public void UpdateItemAmount(ItemObject _item)
    {
        itemsImages[_item].GetComponentInChildren<TextMeshProUGUI>().text = InventoryManager.Instance.GetTotalItemAmount(_item).ToString();
    }

    public void SetItemFloaty(ItemObject _item, bool _isFloaty)
    {
        itemsImages[_item].GetComponent<ImageFloatEffect>().canFloat = _isFloaty;
    }

}
