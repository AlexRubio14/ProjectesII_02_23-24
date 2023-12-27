using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FirstTimeQuestController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleMissionText;
    [SerializeField]
    private TextMeshProUGUI missionResumeText;

    [Space, Header("Requirements"), SerializeField]
    private LayoutGroup requirementsLayout;
    private List<Image> prizeImages;
    private List<TextMeshProUGUI> prizeTexts;

    [Space, Header("Rewards"), SerializeField]
    private LayoutGroup rewardLayout;
    private List<TextMeshProUGUI> rewardList;

    [Space, SerializeField]
    private DialogueController dialogue;

    private void OnEnable()
    {
        QuestObject quest = QuestManager.Instance.GetCurrentQuest();

        SetValues(quest);
        dialogue.StartDialogue();
    }

    private void SetValues(QuestObject _quest)
    {
        titleMissionText.text = _quest.questID.ToString() + ": " + _quest.questName.ToString();
        missionResumeText.text = _quest.questResume;

        SetRequirementValues(_quest);
        SetRewardValues(_quest);

        dialogue.dialogues = _quest.questDialogue;
        dialogue.gameObject.SetActive(true);
    }

    private void SetRequirementValues(QuestObject _quest)
    {
        prizeImages = new List<Image>();
        prizeTexts = new List<TextMeshProUGUI>();

        Dictionary<ItemObject, short> inventory = InventoryManager.Instance.GetAllItems();

        foreach (KeyValuePair<ItemObject, short> item in _quest.neededItems)
        {
            GameObject newObj = new GameObject("Requirement Image");

            Image newImage = newObj.AddComponent<Image>();
            prizeImages.Add(newImage);
            newImage.sprite = item.Key.c_PickableSprite;

            newObj.transform.SetParent(requirementsLayout.transform);

            newObj = new GameObject("Requirement Text");
            TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
            prizeTexts.Add(newText);
            newText.text = inventory[item.Key] + " / " + item.Value;
            newText.enableAutoSizing = true;
            newText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            newText.verticalAlignment = VerticalAlignmentOptions.Middle;

            newObj.transform.SetParent(requirementsLayout.transform);
        }
    }

    private void SetRewardValues(QuestObject _quest)
    {
        rewardList = new List<TextMeshProUGUI>();
        foreach (KeyValuePair<ScriptableObject, QuestObject.RewardType> item in _quest.rewards)
        {
            GameObject newObj = new GameObject("Reward Text");
            TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
            newText.enableAutoSizing = true;

            switch (item.Value)
            {
                case QuestObject.RewardType.UPGRADE:
                    newText.text = ((UpgradeObject)item.Key).UpgradeName;
                    break;
                case QuestObject.RewardType.NEW_QUEST:
                    QuestObject quest = (QuestObject)item.Key;
                    newText.text = quest.questID + ": " + quest.name;
                    break;
                default:
                    break;
            }
            
            newObj.transform.SetParent(rewardLayout.transform);

            rewardList.Add(newText);
        }
    }
}
