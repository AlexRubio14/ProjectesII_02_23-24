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

        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);

        MenuControlsHint.Instance.UpdateHintControls(neededControls, null, MenuControlsHint.HintsPos.BOTTOM_RIGHT);
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        MenuControlsHint.Instance.UpdateHintControls(null);
        TimeManager.Instance.ResumeGame();

        boss.SetActive(true);

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        Destroy(this);
    }
}
