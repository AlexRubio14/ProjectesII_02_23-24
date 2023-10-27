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
    private Dictionary<ItemController.ItemType, Sprite> itemsImages;

    [Space, SerializeField]
    private float timeToDisapear;
    
    private void OnEnable()
    {
        Invoke("DisableNotification", timeToDisapear);
    }

    public void SetType(ItemController.ItemType _itemType, short _itemAmount)
    {
        c_itemImage.sprite = Resources.Load<Sprite>("Minerals/Textures/" + _itemType.ToString());
        string currentItemAmountSign = "+"; 
        if (_itemAmount < 0)
            currentItemAmountSign = "";
        c_text.text = currentItemAmountSign + _itemAmount + " " + _itemType.ToString();


    }

    private void DisableNotification()
    {
        Destroy(gameObject);
    }
}

