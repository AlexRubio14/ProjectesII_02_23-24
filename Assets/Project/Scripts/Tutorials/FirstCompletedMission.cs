using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstCompletedMission : Tutorial
{
    [SerializeField]
    private SelectMissionTutorial selectMissionTutorial;

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(tutorialkey) && PlayerPrefs.GetInt(tutorialkey) == 1)
        {
            Destroy(this);
            return;
        }
        else
        {
            TutorialMethod();
        }
    }

    protected override void TutorialMethod()
    {
        dialogueController.dialogues = dialogues;

        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();
    }

    protected override void EndTutorial()
    {

        dialogueController.onDialogueEnd -= EndTutorial;    

        PlayerPrefs.SetInt(tutorialkey, 1);

        Button[] mission = FindObjectsOfType<Button>(); 
        List<Button> btList = new List<Button>();
        foreach(Button bt in mission)
        {
            QuestCard questCard = bt.GetComponent<QuestCard>();
            

            if(bt.gameObject.activeInHierarchy && !questCard || 
                questCard && questCard.currentQuest.questID != 4)   
            {
                btList.Add(bt);
                bt.interactable = false;
            }
            else if(questCard && questCard.currentQuest.questID == 4)
            {
                bt.Select();
            }
        }

        selectMissionTutorial.buttons = btList;
    }
}
