using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemIconController : MonoBehaviour
{
    [SerializeField]
    private Image c_spriteImage;
    [SerializeField]
    private TextMeshProUGUI c_totalItemsText;

    private ItemObject c_item;

    public void LoadItem(ItemObject _itemType, short _amount)
    {
        c_item = _itemType;
        c_spriteImage.sprite = c_item.c_PickableSprite;
        RefreshItemData(_amount);
    }

    public void RefreshItemData(short _amount)
    {
        //Poner un x(X) en el numero del item
        c_totalItemsText.text = "x" + _amount;

        if (_amount == 0)
        {
            c_spriteImage.color = new Color(1, 1, 1, 0.6f);
        }
        else
        {
            c_spriteImage.color = Color.white;
        }

    }

    public ItemObject GetItemType() { return c_item; }

}
