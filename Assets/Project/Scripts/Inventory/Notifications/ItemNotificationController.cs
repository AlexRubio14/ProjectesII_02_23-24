using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject c_notificationPrefab;

    [SerializeField]
    private LayoutGroup notificationLayout;

    private List<NotificationController> l_notificationList;

    private void Awake()
    {
        l_notificationList = new List<NotificationController>();
    }

    private void AddNewItem(ItemObject _itemType, short _itemAmount)
    {

        foreach (NotificationController item in l_notificationList)
        {
            if (!item)
            {
                l_notificationList.Remove(item);
                AddNewItem(_itemType, _itemAmount);
                return;
            }else if (item.GetItemType() == _itemType)
            {
                item.AddItemAmount(_itemAmount);
                return;
            }
        }

        CreateItemNotification(_itemType, _itemAmount);

    }
    private void CreateItemNotification(ItemObject _itemType, short _itemAmount)
    {
        NotificationController newItem = Instantiate(c_notificationPrefab, notificationLayout.transform).GetComponent<NotificationController>();
        l_notificationList.Add(newItem.GetComponent<NotificationController>());

        newItem.SetType(_itemType, _itemAmount);

    }

    

    private void OnEnable()
    {
        InventoryManager.Instance.obtainItemAction += AddNewItem;
    }


    private void OnDisable()
    {
        InventoryManager.Instance.obtainItemAction -= AddNewItem;
    }





}
