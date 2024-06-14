using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private QuestObject[] allQuests;

    private int selectedQuestID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        allQuests = Resources.LoadAll<QuestObject>("Quests");
        LoadQuests();
        selectedQuestID = 0;
    }

    public List<QuestObject> GetAllObtainedQuests()
    {
        List<QuestObject> obtainedQuests = new List<QuestObject>();

        foreach (QuestObject item in allQuests)
        {
            if (item.obtainedQuest)
            {
                obtainedQuests.Add(item);
            }
        }

        return obtainedQuests;
    }

    public List<QuestObject> GetAllCompletedQuests()
    {
        List<QuestObject> completedQuests = new List<QuestObject>();

        foreach (QuestObject item in allQuests)
        {
            if (item.completedQuest)
            {
                completedQuests.Add(item);
            }
        }

        return completedQuests;
    }

    public List<QuestObject> GetObtainedButNotCompletedQuests()
    {
        List<QuestObject> obtainedQuests = new List<QuestObject>();

        foreach (QuestObject item in allQuests)
        {
            if (item.obtainedQuest && !item.completedQuest)
            {
                obtainedQuests.Add(item);
            }
        }

        return obtainedQuests;
    }

    public QuestObject GetSelectedQuest()
    {

        for (int i = 0; i < allQuests.Length; i++)
        {
            if (allQuests[i].questID == selectedQuestID && !allQuests[i].completedQuest)
            {
                return allQuests[i];
            }
        }

        return null;
    }

    public void SetSelectedQuest(int _id)
    {
        selectedQuestID = _id;
    }

    public void ResetQuests()
    {
        QuestObject[] quests = Resources.LoadAll<QuestObject>("Quests");
        foreach (QuestObject item in quests)
        {
            item.obtainedQuest = false;
            item.completedQuest = false;
            item.newQuest = true;
        }

        selectedQuestID = 0;

        SaveQuests();
    }

    private void LoadQuests()
    {
        foreach (QuestObject item in allQuests)
        {
            item.obtainedQuest = PlayerPrefs.HasKey("Quest_" + item.questID + "_obtained") && PlayerPrefs.GetInt("Quest_" + item.questID + "_obtained") == 1;
            item.completedQuest = PlayerPrefs.HasKey("Quest_" + item.questID + "_completed") && PlayerPrefs.GetInt("Quest_" + item.questID + "_completed") == 1;
            item.newQuest = !PlayerPrefs.HasKey("Quest_" + item.questID + "_new") || PlayerPrefs.GetInt("Quest_" + item.questID + "_new") == 1;

        }
    }
    public void SaveQuests()
    {
        foreach (QuestObject item in allQuests)
        {
            PlayerPrefs.SetInt("Quest_" + item.questID + "_obtained", item.obtainedQuest ? 1 : 0);
            PlayerPrefs.SetInt("Quest_" + item.questID + "_completed", item.completedQuest ? 1 : 0);
            PlayerPrefs.SetInt("Quest_" + item.questID + "_new", item.newQuest ? 1 : 0);
        }

        PlayerPrefs.Save();
    }


    public bool CanCompleteSomeQuest()
    {
        foreach (QuestObject item in allQuests)
        {
            if (item.obtainedQuest && !item.completedQuest)
            {
                if (CanCompleteQuest(item))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CanCompleteQuest(QuestObject _quest)
    {
        Dictionary<ItemObject, short> inventory = InventoryManager.Instance.GetAllItems();
        foreach (KeyValuePair<ItemObject, short> item in _quest.neededItems)
        {
            if (inventory[item.Key] < item.Value)
                return false;
        }

        return true;
    }


}
