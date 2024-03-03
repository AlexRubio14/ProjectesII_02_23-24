using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMissionTutorial : Tutorial
{
    [SerializeField]
    private QuestObject firstMission;
    [SerializeField]
    private Canvas selectButtonCanvas;

    public List<Button> buttons;

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(tutorialkey) && PlayerPrefs.GetInt(tutorialkey) == 1)
        {
            Destroy(this);
            return;
        }
        else if(firstMission.completedQuest)
        {
            TutorialMethod();
        }
    }

    protected override void TutorialMethod()
    {
        dialogueController.dialogues = dialogues;
        selectButtonCanvas.gameObject.SetActive(true);

        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();
    }

    protected override void EndTutorial()
    {
        selectButtonCanvas.gameObject.SetActive(false);

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        foreach(Button bt in buttons)
        {
            if (bt)
            {
                bt.interactable = true;
            }
        }
    }
}
