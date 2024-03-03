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

    [SerializeField]
    private float timeToWaitForFloat;



    // Start is called before the first frame update
    void Start()
    {
        prizeImages = new List<Image>();
        prizeTexts = new List<TextMeshProUGUI>();

        SetupQuestCanvas(QuestManager.Instance.GetSelectedQuest());
    }

    public void SetupQuestCanvas(QuestObject _quest)
    {
        currentQuest = _quest;

        if (!currentQuest)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        questTitleText.text = currentQuest.questName;

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            Image newImage = Instantiate(needImagePrefab, layoutQuest.transform).GetComponent<Image>();
            prizeImages.Add(newImage);
            newImage.sprite = item.Key.PickableSprite;
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

            if(item.Key == _itemType)
            {
                for(int i = 0; i < prizeImages.Count; i++)
                {
                    if (prizeImages[i].sprite == _itemType.PickableSprite) 
                    {
                        StopCoroutine("StopFloating");

                        ImageFloatEffect temp = prizeImages[i].GetComponent<ImageFloatEffect>();
                        temp.canFloat = true;
                        StartCoroutine("StopFloating", temp);
                    }
                }
            }

            prizeTexts[index].text = InventoryManager.Instance.GetTotalItemAmount(item.Key) + " / " + item.Value;
                        
            index++;
        }
    }

    private IEnumerator StopFloating(ImageFloatEffect imageFloatEffect)
    {
        yield return new WaitForSeconds(timeToWaitForFloat);
        imageFloatEffect.canFloat = false;
    }

    public void RemoveCurrentQuest()
    {
        for (int i = 0; i < prizeImages.Count; i++)
        {
            Destroy(prizeImages[i].gameObject);
            Destroy(prizeTexts[i].gameObject);
        }

        prizeImages.Clear();
        prizeTexts.Clear();
        
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
