using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject c_notificationPrefab;

    [SerializeField]
    private Dictionary<ItemController.ItemType, Sprite> l_itemsSprite;

    [SerializeField]
    private float notificationMaxYSpawn;
    [SerializeField]
    private float notificationOffset;

    private List<RectTransform> l_notificationList;

    [Space, SerializeField]
    private GameObject c_inventory; //ESTO NO HA DE ESTAR AQUI, ES SOLO PARA PRUEBAS
    private void Awake()
    {
        l_notificationList = new List<RectTransform>();
    }

    private void Update()
    {
        PlaceListItems();

        if (Input.GetKeyDown(KeyCode.I))
        {
            c_inventory.SetActive(!c_inventory.activeInHierarchy);
        }
    }

    private void CreateItemNotification(ItemController.ItemType _itemType, short _itemAmount)
    {
        GameObject newItem = Instantiate(c_notificationPrefab, transform);
        l_notificationList.Add(newItem.GetComponent<RectTransform>());

        NotificationController notification = newItem.GetComponent<NotificationController>();
        notification.SetType(_itemType, _itemAmount);

    }

    private void PlaceListItems() 
    {
        for (int i = 0; i < l_notificationList.Count; i++)
        {
            if (!l_notificationList[i])
            {
                l_notificationList.RemoveAt(i);
                i--;
                continue;
            }

            if (i < 3)
            {
                l_notificationList[i].gameObject.SetActive(true);
                l_notificationList[i].anchoredPosition = new Vector2(0, notificationMaxYSpawn - (notificationOffset * i));
            }
            else
            {
                l_notificationList[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.onItemChange += CreateItemNotification;
    }


    private void OnDisable()
    {
        EventManager.onItemChange -= CreateItemNotification;
    }





}
