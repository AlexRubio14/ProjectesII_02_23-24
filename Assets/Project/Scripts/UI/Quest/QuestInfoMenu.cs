using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestInfoMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleMissionText;
    [SerializeField]
    private TextMeshProUGUI missionResumeText;

    [Space, Header("Requirements"), SerializeField]
    private LayoutGroup requirementsLayout;
    private List<Image> requirementImages;
    private List<TextMeshProUGUI> requirementTexts;

    [Space, Header("Rewards"), SerializeField]
    private LayoutGroup rewardLayout;
    private List<TextMeshProUGUI> rewardList;

    [Space, SerializeField]
    private DialogueController dialogue;

    private QuestObject currentQuest;

    [SerializeField]
    private TMP_FontAsset fontAsset;

    private void OnDisable()
    {
        RemoveQuestInfo();
    }
    public void SetValues(QuestObject _quest)
    {
        currentQuest = _quest;
        titleMissionText.text = currentQuest.questTitle.ToString() + ": " + currentQuest.questName.ToString();
        missionResumeText.text = currentQuest.questResume;

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

        Dictionary<ItemObject, short> inventory = InventoryManager.Instance.GetAllItems();

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            GameObject newObj = new GameObject("Requirement Image");

            Image newImage = newObj.AddComponent<Image>();
            requirementImages.Add(newImage);
            newImage.sprite = item.Key.c_PickableSprite;

            newObj.transform.SetParent(requirementsLayout.transform);

            newObj = new GameObject("Requirement Text");
            TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
            newText.font = fontAsset; 
            requirementTexts.Add(newText);
            newText.text = inventory[item.Key] + " / " + item.Value;
            newText.enableAutoSizing = true;
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
            GameObject newObj = new GameObject("Reward Text");
            TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
            newText.fontSize = 20;
            newText.font = fontAsset; 

            switch (item.Value)
            {
                case QuestObject.RewardType.UPGRADE:
                    newText.text = ((UpgradeObject)item.Key).UpgradeName;
                    break;
                case QuestObject.RewardType.NEW_QUEST:
                    QuestObject quest = (QuestObject)item.Key;
                    newText.text = quest.questTitle + ": " + quest.questName;
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
}
