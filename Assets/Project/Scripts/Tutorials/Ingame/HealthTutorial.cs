using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealthTutorial : Tutorial
{
    [SerializeField]
    GameObject tutorialCanvas;

    protected override void TutorialMethod()
    {
        StartHealthTutorial();
    }

    private void StartHealthTutorial()
    {
        tutorialCanvas.SetActive(true);

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
        tutorialCanvas.SetActive(false);
        //MenuControlsHint.Instance.UpdateHintControls(null);
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;
        PlayerPrefs.SetInt(tutorialkey, 1);
    }
}
