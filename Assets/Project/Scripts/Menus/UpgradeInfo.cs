using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradeImagePrefab;
    [SerializeField]
    private GameObject upgradeTextPrefab;

    [SerializeField]
    private LayoutGroup layoutUpgrade; 

    [SerializeField]
    private UpgradeObject currentUpgrade;

    [SerializeField]
    private Image upgradeImage;

    [SerializeField]
    private TextMeshProUGUI upgradeText;

    private void Start()
    {
        SetUpgradeLayout(); 
    }

    private void SetUpgradeLayout()
    {
        Dictionary<ItemObject, short> inventoryMap = InventoryManager.Instance.GetAllItems();

        upgradeImage.sprite = currentUpgrade.c_UpgradeSprite;
        upgradeText.text = currentUpgrade.UpgradeName;

        currentUpgrade.Awake();

        foreach (KeyValuePair<ItemObject, short> item in currentUpgrade.prize)
        {
            Instantiate(upgradeImagePrefab, layoutUpgrade.transform).GetComponent<Image>().sprite = item.Key.c_PickableSprite;
            Instantiate(upgradeTextPrefab, layoutUpgrade.transform).GetComponent<TextMeshProUGUI>().text = inventoryMap[item.Key] + " / " + item.Value;
        }
    }

    public void BuyUpgrade(UpgradeObject _upgradeObject)
    {
        if (InventoryManager.Instance.CanBuy(_upgradeObject.prize))
        {
            InventoryManager.Instance.BuyUpgrade(_upgradeObject.prize);
            // UpgradeManager.Instance.
        }
    }

}
