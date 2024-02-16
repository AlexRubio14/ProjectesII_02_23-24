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
        MenuControlsHint.Instance.UpdateHintControls(null);
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

        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);

        MenuControlsHint.Instance.UpdateHintControls(neededControls, null, MenuControlsHint.HintsPos.BOTTOM_RIGHT);
        TimeManager.Instance.PauseGame();
    }
}
