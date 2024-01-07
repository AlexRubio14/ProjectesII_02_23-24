using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestCanvasController : MonoBehaviour
{
    [SerializeField]
    private GameObject needImagePrefab;
    [SerializeField]
    private GameObject needTextPrefab;

    [SerializeField]
    private TextMeshProUGUI questTitleText;
    [SerializeField]
    private LayoutGroup layoutQuest;

    private List<Image> prizeImages;
    private List<TextMeshProUGUI> prizeTexts;

    private QuestObject currentQuest;

    // Start is called before the first frame update
    void Start()
    {
        prizeImages = new List<Image>();
        prizeTexts = new List<TextMeshProUGUI>();

        SetupQuestCanvas();
    }

    private void SetupQuestCanvas()
    {
        currentQuest = QuestManager.Instance.GetSelectedQuest();

        if (!currentQuest)
        {
            gameObject.SetActive(false);
            return;
        }
        questTitleText.text = currentQuest.questName;

        for (int i = 0; i < currentQuest.neededItems.Count; i++)
        {
            

        }

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            Image newImage = Instantiate(needImagePrefab, layoutQuest.transform).GetComponent<Image>();
            prizeImages.Add(newImage);
            newImage.sprite = item.Key.c_PickableSprite;
            TextMeshProUGUI newText = Instantiate(needTextPrefab, layoutQuest.transform).GetComponent<TextMeshProUGUI>();
            prizeTexts.Add(newText);
        }

        UpdateQuestCanvas();

    }

    private void UpdateQuestCanvas(ItemObject _itemType = null, short _itemAmount = 0)
    {
        int index = 0;

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
  
            prizeTexts[index].text = InventoryManager.Instance.GetTotalItemAmount(item.Key) + " / " + item.Value;

            index++;
        }

    }

    private void OnEnable()
    {
        InventoryManager.Instance.obtainItemAction += UpdateQuestCanvas;   
    }

    private void OnDisable()
    {
        InventoryManager.Instance.obtainItemAction -= UpdateQuestCanvas;

    }

}
