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

    [SerializeField]
    private Button buyButton;
    private TextMeshProUGUI buttonText;

    private List<Image> prizeImages;
    private List<TextMeshProUGUI> prizeTexts;

    private void Awake()
    {
        buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
    }



    private void SetUpLayout()
    {
        if (prizeImages != null || prizeTexts != null)
            return;

        prizeImages = new List<Image>();
        prizeTexts = new List<TextMeshProUGUI>();

        upgradeImage.sprite = currentUpgrade.c_UpgradeSprite;
        upgradeText.text = currentUpgrade.UpgradeName;

        for (int i = 0; i < currentUpgrade.prize.Count; i++)
        {
            Image newImage = Instantiate(upgradeImagePrefab, layoutUpgrade.transform).GetComponent<Image>();
            prizeImages.Add(newImage);

            TextMeshProUGUI newText = Instantiate(upgradeTextPrefab, layoutUpgrade.transform).GetComponent<TextMeshProUGUI>();
            prizeTexts.Add(newText);
        }

        UpdatePrizeValues();

    }

    private void UpdatePrizeValues()
    {
        Dictionary<ItemObject, short> inventoryMap = InventoryManager.Instance.GetAllItems();
        int index = 0;

        foreach (KeyValuePair<ItemObject, short> item in currentUpgrade.prize)
        {
            prizeImages[index].sprite = item.Key.PickableSprite;

            prizeTexts[index].text = inventoryMap[item.Key] + " / " + item.Value;

            index++;
        }
    }

    private void SetButtonValues()
    {
        //Si tenemos la mejora desactivamos el boton y ponemos Sold
        //Si no la tenemos nos pondra Buy y agregamos la funcion en el boton
        if (UpgradeManager.Instance.CheckObtainedUpgrade(currentUpgrade))
        {
            buyButton.interactable = false;
            buttonText.text = "Sold";
        }
        else
        {
            buyButton.onClick.AddListener(BuyUpgrade);
            buttonText.text = "Buy";
        }
    }

    public void BuyUpgrade()
    {
        if (InventoryManager.Instance.CanBuy(currentUpgrade.prize))
        {
            InventoryManager.Instance.Buy(currentUpgrade.prize);
            UpgradeManager.Instance.ObtainUpgrade(currentUpgrade);
        }
        else
        {
            SetPrizesRed();
        }

        SetButtonValues();
        UpdatePrizeValues();
    }

    private void SetPrizesRed()
    {
        Dictionary<ItemObject, short> inventoryMap = InventoryManager.Instance.GetAllItems();

        foreach (KeyValuePair<ItemObject, short> item in currentUpgrade.prize)
        {
            if (item.Value > inventoryMap[item.Key])
            {
                for (int i = 0; i < prizeTexts.Count; i++)
                {
                    if (item.Key.PickableSprite == prizeImages[i].sprite)
                    {
                        prizeImages[i].color = Color.red;
                        prizeTexts[i].color = Color.red;
                    }
                    
                }
            }
        }

        


        Invoke("SetPrizesNormalColor", 1.5f);
    }

    private void SetPrizesNormalColor()
    {
        for (int i = 0; i < prizeTexts.Count; i++)
        {
            prizeImages[i].color = Color.white;
            prizeTexts[i].color = Color.white;
        }
    }


    private void OnEnable()
    {
        SetUpLayout();
        SetButtonValues();
    }
    private void OnDisable()
    {
        buyButton.onClick.RemoveAllListeners();
    }



}
