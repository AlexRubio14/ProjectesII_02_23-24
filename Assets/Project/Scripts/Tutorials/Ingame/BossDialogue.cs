using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDialogue : Tutorial
{

    [SerializeField]
    private GameObject boss;

    protected override void TutorialMethod()
    {
        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        TimeManager.Instance.ResumeGame();

        boss.SetActive(true);

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        Destroy(this);
    }
}
