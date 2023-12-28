using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayQuestController : MonoBehaviour
{
    [Header("First Time Quest"), SerializeField]
    private QuestInfoMenu firstTimeQuest;

    [Space, Header("Check List"), SerializeField]
    private CheckQuestController checkController;

    [Space, Header("New Quests")]
    public List<QuestObject> newQuests;

    public bool checkDisplayNewQuests = false;

    private void Start()
    {
        newQuests = new List<QuestObject>();

        QuestObject selectedQuest = QuestManager.Instance.GetSelectedQuest();
        if (selectedQuest && !selectedQuest.obtainedQuest)
        {
            firstTimeQuest.gameObject.SetActive(true);
            firstTimeQuest.SetValues(selectedQuest);
        }
    }

    private void Update()
    {
        CheckForNewQuests();
    }
    
    private void CheckForNewQuests()
    {
        if (checkDisplayNewQuests && !firstTimeQuest.gameObject.activeInHierarchy)
        {
            if (newQuests.Count <= 0)
            {
                checkDisplayNewQuests = false;
                return;
            }
            else if (newQuests[0].obtainedQuest)
            {
                newQuests.RemoveAt(0);
                return;
            }

            firstTimeQuest.gameObject.SetActive(true);
            firstTimeQuest.SetValues(newQuests[0]);
        }
    }

    public void DisplayQuestList()
    {
        checkController.gameObject.SetActive(true);
        checkController.DisplayQuestCardList();
    }



}
