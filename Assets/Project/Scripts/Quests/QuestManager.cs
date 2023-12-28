using System.Collections;
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

}
