using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUIController : MonoBehaviour
{

    [SerializeField]
    private GameObject itemPrefab;

    [Space, Header("UI") ,SerializeField]
    private TextMeshProUGUI c_totalWeightText;
    [SerializeField]
    private RectTransform c_background;

    [Space, Header("Background"), SerializeField]
    private float sizePerItem;
    [SerializeField]
    private float topOffset;

    [Space, Header("Items"), SerializeField]
    private Vector2 starterPos;

    [SerializeField]
    private Vector2 posOffset;

    [Space,SerializeField]
    private int maxItemPerRow;

    private List<RectTransform> l_itemsDisplayed;
    private List<InventoryItemIconController> l_itemIcon;
    private Dictionary<ItemObject, short> l_items;


    private void Awake()
    {
        l_itemsDisplayed = new List<RectTransform>();
        l_itemIcon = new List<InventoryItemIconController>();
    }

    private void FixedUpdate()
    {
        RefreshInventoryItemAmount();
        CalculateWeight();
    }


    private void OnEnable()
    {
        LoadItemList();
    }

    private void OnDisable()
    {
        //borrar todos los objetos de la lista de l_itemsDisplayed
        foreach (RectTransform item in l_itemsDisplayed)
        {
            Destroy(item.gameObject);
        }
        l_itemsDisplayed.Clear();
        l_itemIcon.Clear();
    }


    private void LoadItemList()
    {
        l_items = InventoryManager.Instance.GetRunItems();



        //Resize al background
        c_background.sizeDelta = new Vector2(
            sizePerItem * maxItemPerRow,
            sizePerItem * Mathf.CeilToInt(Mathf.Clamp(l_items.Count, 1, Mathf.Infinity) / maxItemPerRow) + topOffset
            );

        //Meto todos los objetos 
        //Y me los guardo en la lista de rect transform y otra con los scripts de cada item
        foreach (KeyValuePair<ItemObject, short> item in l_items)
        {
            //Quitar los que sean 0
            if (item.Value <= 0)
                continue;

            GameObject currentItem = Instantiate(itemPrefab, transform);
            l_itemsDisplayed.Add(currentItem.GetComponent<RectTransform>());
            l_itemIcon.Add(currentItem.GetComponent<InventoryItemIconController>());
            l_itemIcon[l_itemIcon.Count - 1].LoadItem(item.Key, item.Value);
        }

        //Coloco los elementos de la lista de los items que tengo
        for (int i = 0; i < l_itemsDisplayed.Count; i++)
        {
            l_itemsDisplayed[i].anchoredPosition = new Vector2(
                starterPos.x + posOffset.x * (i % maxItemPerRow),
                starterPos.y - posOffset.y * Mathf.FloorToInt(i / maxItemPerRow)
                );
        }
    }

    private void RefreshInventoryItemAmount()
    {
        foreach (InventoryItemIconController item in l_itemIcon)
        {
            if (l_items.ContainsKey(item.GetItemType()))
                item.RefreshItemData(l_items[item.GetItemType()]);
        }
    }
    private void CalculateWeight()
    {
        float weight = 0;

        foreach (KeyValuePair<ItemObject, short> item in l_items)
        {
            weight += item.Key.weight * item.Value;
        }

        c_totalWeightText.text = weight.ToString("F1");
    }

    

}
