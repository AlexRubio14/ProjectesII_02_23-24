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
    
    private void OnEnable()
    {
        Invoke("DisableNotification", timeToDisapear);
    }

    public void SetType(ItemObject _item, short _itemAmount)
    {
        c_itemImage.sprite = _item.c_PickableSprite;
        string currentItemAmountSign = "+"; 
        if (_itemAmount < 0)
            currentItemAmountSign = "";
        c_text.text = currentItemAmountSign + _itemAmount + " " + _item.name;


    }

    private void DisableNotification()
    {
        Destroy(gameObject);
    }
}

