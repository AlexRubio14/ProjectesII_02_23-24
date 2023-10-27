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

    private ItemController.ItemType type;

    public void LoadItem(ItemController.ItemType _itemType, short _amount)
    {
        //Cargar el sprite que toque
        c_spriteImage.sprite = Resources.Load<Sprite>("Minerals/Textures/" + _itemType.ToString());
        type = _itemType;

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


    public ItemController.ItemType GetItemType() { return type; }
}
