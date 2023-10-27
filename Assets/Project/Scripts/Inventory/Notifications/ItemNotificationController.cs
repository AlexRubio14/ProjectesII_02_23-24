using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject notificationPrefab;

    [SerializeField]
    private Dictionary<ItemController.ItemType, Sprite> itemsSprite;

    [SerializeField]
    private float notificationMaxYSpawn;
    [SerializeField]
    private float notificationOffset;

    private List<RectTransform> notificationList;

    [Space, SerializeField]
    private GameObject inventory; //ESTO NO HA DE ESTAR AQUI, ES SOLO PARA PRUEBAS
    private void Awake()
    {
        notificationList = new List<RectTransform>();
    }

    private void Update()
    {
        PlaceListItems();

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.SetActive(!inventory.activeInHierarchy);
        }
    }

    private void CreateItemNotification(ItemController.ItemType _itemType, short _itemAmount)
    {
        GameObject newItem = Instantiate(notificationPrefab, transform);
        notificationList.Add(newItem.GetComponent<RectTransform>());

        NotificationController notification = newItem.GetComponent<NotificationController>();
        notification.SetType(_itemType, _itemAmount);

    }

    private void PlaceListItems() 
    {
        for (int i = 0; i < notificationList.Count; i++)
        {
            if (!notificationList[i])
            {
                notificationList.RemoveAt(i);
                i--;
                continue;
            }

            if (i < 3)
            {
                notificationList[i].gameObject.SetActive(true);
                notificationList[i].anchoredPosition = new Vector2(0, notificationMaxYSpawn - (notificationOffset * i));
            }
            else
            {
                notificationList[i].gameObject.SetActive(false);
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
