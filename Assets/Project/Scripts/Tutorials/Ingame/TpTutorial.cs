using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpTutorial : Tutorial
{
    protected override void TutorialMethod()
    {
        StartTpTutorial();
    }

    protected override void EndTutorial()
    {
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        Destroy(this);
    }

    private void StartTpTutorial()
    {
        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();
        TimeManager.Instance.PauseGame();
    }
}
