using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealthTutorial : Tutorial
{
    [SerializeField]
    private GameObject tutorialCanvas;
    [SerializeField]
    private GameObject tpTutorial;

    protected override void TutorialMethod()
    {
        StartHealthTutorial();
    }

    private void StartHealthTutorial()
    {
        tutorialCanvas.SetActive(true);

        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.onDialogueLineStart += OnDialogueLineStarted;

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
        tutorialCanvas.SetActive(false);
        tpTutorial.SetActive(false);
        //MenuControlsHint.Instance.UpdateHintControls(null);
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;
        dialogueController.onDialogueLineStart -= OnDialogueLineStarted;

        PlayerPrefs.SetInt(tutorialkey, 1);
        
        Destroy(this);
    }


    private void OnDialogueLineStarted(int _dialogueLineID)
    {
        if (_dialogueLineID == 2)
        {
            tutorialCanvas.SetActive(false);
            tpTutorial.SetActive(true);
        }
    }

}
