using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestInfo : MonoBehaviour
{

    [SerializeField]
    private GameObject needImagePrefab;
    [SerializeField]
    private GameObject needTextPrefab;

    [SerializeField]
    private LayoutGroup layoutQuest;
    [SerializeField]
    private TextMeshProUGUI titleText;

    private List<Image> prizeImages;
    private List<TextMeshProUGUI> prizeTexts;

    private QuestObject currentQuest;

    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private GameObject nextQuestMenu;

    // Start is called before the first frame update
    void OnEnable()
    {
        SetInfoValues(QuestManager.Instance.GetCurrentQuest());

    }

    private void OnDisable()
    {
        foreach (Image item in prizeImages)
        {
            Destroy(item.gameObject);
        }
        
        foreach (TextMeshProUGUI item in prizeTexts)
        {
            Destroy(item.gameObject);
        }
    }


    public void SetInfoValues(QuestObject _quest)
    {
        currentQuest = _quest;

        prizeImages = new List<Image>();
        prizeTexts = new List<TextMeshProUGUI>();

        titleText.text = currentQuest.questIntroduction;


        for (int i = 0; i < _quest.neededItems.Count; i++)
        {
            Image newImage = Instantiate(needImagePrefab, layoutQuest.transform).GetComponent<Image>();
            prizeImages.Add(newImage);

            TextMeshProUGUI newText = Instantiate(needTextPrefab, layoutQuest.transform).GetComponent<TextMeshProUGUI>();
            prizeTexts.Add(newText);
        }

        UpdatePrizeValues();
    }

    private void UpdatePrizeValues()
    {
        Dictionary<ItemObject, short> inventoryMap = InventoryManager.Instance.GetAllItems();
        int index = 0;

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            prizeImages[index].sprite = item.Key.c_PickableSprite;

            prizeTexts[index].text = inventoryMap[item.Key] + " / " + item.Value;

            index++;
        }
    }

    public void QuestButton()
    {

        gameObject.SetActive(true);


        currentQuest = QuestManager.Instance.GetCurrentQuest();
        if (InventoryManager.Instance.CanBuy(currentQuest.neededItems))
        {
            yesButton.interactable = true;
        }
        else
        {
            yesButton.interactable = false;
        }
    }

    public void YesButton()
    {
        InventoryManager.Instance.Buy(currentQuest.neededItems);
        QuestManager.Instance.EndCurrentQuest();
        nextQuestMenu.SetActive(true);
        gameObject.SetActive(false);

    }

    public void NoButton()
    {
        gameObject.SetActive(false);
    }

    
}
