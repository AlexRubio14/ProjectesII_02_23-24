using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealthTutorial : Tutorial
{
    [SerializeField]
    Canvas tutorialCanvas;

    protected override void TutorialMethod()
    {
        PlayerPrefs.SetInt(tutorialkey, 1);

        StartHealthTutorial();
    }

    private void StartHealthTutorial()
    {
        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();
        
        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);

        MenuControlsHint.Instance.UpdateHintControls(neededControls);
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        //tutorialCanvas.enabled = false;
        //MenuControlsHint.Instance.UpdateHintControls(null);
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;
    }
}
