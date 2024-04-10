using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

public class QuestInfoMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleMissionText;

    [Space, Header("Requirements"), SerializeField]
    private LayoutGroup requirementsLayout;
    private List<Image> requirementImages;
    private List<TextMeshProUGUI> requirementTexts;
    public bool canItemsFloat;

    [Space, Header("Rewards"), SerializeField]
    private LayoutGroup rewardLayout;
    private List<TextMeshProUGUI> rewardList;
    [SerializeField]
    private float rewardFontSize;
    [SerializeField]
    private bool showCurrentItems;

    [Space, SerializeField]
    private DialogueController dialogue;
    public List<Button> onFirstQuestClosed; //El boton 0 sera en caso de que sea la primera vez que obtenemos una mision, el 1 sera el else
    private QuestObject currentQuest;

    [Space, SerializeField]
    private TMP_FontAsset fontAsset;

    private DisplayQuestController displayQuestController;

   

    private void Awake()
    {
        displayQuestController = GetComponentInParent<DisplayQuestController>();
    }

    private void OnDisable()
    {
        RemoveQuestInfo();
    }
    public void SetValues(QuestObject _quest)
    {
        currentQuest = _quest;
        titleMissionText.text = currentQuest.questTitle.ToString() + ": " + currentQuest.questName.ToString();

        SetRequirementValues();
        SetRewardValues();

        if (!dialogue)
            return;

        dialogue.dialogues = currentQuest.questDialogue;
        dialogue.gameObject.SetActive(true);
        dialogue.StartDialogue();
    }
    private void SetRequirementValues()
    {
        requirementImages = new List<Image>();
        requirementTexts = new List<TextMeshProUGUI>();

        Dictionary<ItemObject, short> inventory = InventoryManager.Instance.GetSavetems();

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            GameObject newObj = new GameObject("Requirement Image");

            Image newImage = newObj.AddComponent<Image>();
            requirementImages.Add(newImage);
            newImage.sprite = item.Key.PickableSprite;
            newImage.preserveAspect = true;

            newObj.transform.SetParent(requirementsLayout.transform);
            ImageFloatEffect floatFX = newObj.AddComponent<ImageFloatEffect>();
            floatFX.canFloat = canItemsFloat;
            floatFX.speed = 1.4f;
            floatFX.maxExpand = 1.5f;
            floatFX.minExpand = 1.1f;


            newObj = new GameObject("Requirement Text");
            TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
            newText.font = fontAsset; 
            requirementTexts.Add(newText);
            if (showCurrentItems)
                newText.text = item.Key.ItemName + "   " + inventory[item.Key] + " / " + item.Value;
            else
                newText.text = item.Key.ItemName + " x " + item.Value;
            newText.enableAutoSizing = true;
            newText.fontSizeMin = 14;
            newText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            newText.verticalAlignment = VerticalAlignmentOptions.Middle;
            newText.enableWordWrapping = false;

            newObj.transform.SetParent(requirementsLayout.transform);
        }
    }
    private void SetRewardValues()
    {
        rewardList = new List<TextMeshProUGUI>();
        foreach (KeyValuePair<ScriptableObject, QuestObject.RewardType> item in currentQuest.rewards)
        {
            if(item.Value == QuestObject.RewardType.NEW_QUEST) {
                continue; 
            }
            GameObject newObj = new GameObject("Reward Text");
            TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
            newText.fontSize = rewardFontSize;
            newText.font = fontAsset;
            newText.alignment = TextAlignmentOptions.Justified;
            switch (item.Value)
            {
                case QuestObject.RewardType.UPGRADE:
                    newText.text = ((UpgradeObject)item.Key).UpgradeName + ": " + ((UpgradeObject)item.Key).UpgradeDescription;
                    break;
                case QuestObject.RewardType.POWER_UP:
                    newText.text = ((ItemObject)item.Key).ItemName;
                    break;
                default:
                    break;
            }
            
            newObj.transform.SetParent(rewardLayout.transform);

            rewardList.Add(newText);
        }
    }

    public void ObtainCurrentQuest()
    {
        currentQuest.obtainedQuest = true;

        QuestManager.Instance.SaveQuests();
    }
    public void RemoveQuestInfo()
    {
        for (int i = 0; i < requirementImages.Count; i++)
        {
            Destroy(requirementImages[i].gameObject);
            Destroy(requirementTexts[i].gameObject);
        }

        requirementImages.Clear();
        requirementTexts.Clear();

        foreach (TextMeshProUGUI item in rewardList)
        {
            Destroy(item.gameObject);
        }

        rewardList.Clear();
    }


    public void SelectEndFirtstTimeButton()
    {
        if (currentQuest.questID == 0 )
        {
            onFirstQuestClosed[0].Select();
        }
    }


}
