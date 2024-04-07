using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject notificationPrefab;

    [SerializeField]
    private LayoutGroup notificationLayout;

    private List<NotificationController> notificationList;

    private void Awake()
    {
        notificationList = new List<NotificationController>();
    }

    private void AddNewItem(ItemObject _itemType, short _itemAmount)
    {

        foreach (NotificationController item in notificationList)
        {
            if (!item)
            {
                notificationList.Remove(item);
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
        NotificationController newItem = Instantiate(notificationPrefab, notificationLayout.transform).GetComponent<NotificationController>();
        notificationList.Add(newItem.GetComponent<NotificationController>());

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
