using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NotificationController : MonoBehaviour
{
    
    [SerializeField]
    private Image c_itemImage;
    [SerializeField]
    private TextMeshProUGUI c_text;

    [Space, SerializeField]
    private float timeToDisapear;
    private float timeToDesapearWaited = 0;

    private ItemObject notificationType;
    private short currentAmount;


    private void Update()
    {
        timeToDesapearWaited += Time.deltaTime * TimeManager.Instance.timeParameter;
        if (timeToDesapearWaited >= timeToDisapear)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(ItemObject _item, short _itemAmount)
    {
        notificationType = _item;
        currentAmount = _itemAmount;

        c_itemImage.sprite = notificationType.c_PickableSprite;
        string currentItemAmountSign = "+"; 
        if (currentAmount <= 0)
            currentItemAmountSign = "";
        c_text.text = currentItemAmountSign + currentAmount + " " + notificationType.ItemName;


    }

    public ItemObject GetItemType()
    {
        return notificationType;
    }

    public void AddItemAmount(short _itemAmount)
    {
        currentAmount += _itemAmount;
        timeToDesapearWaited = 0;

        string currentItemAmountSign = "+";
        if (currentAmount <= 0)
            currentItemAmountSign = "";

        c_text.text = currentItemAmountSign + currentAmount + " " + notificationType.ItemName;
    }

}

