using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTutorial : Tutorial
{
    [SerializeField]
    private Canvas boostCanvasTutorial;

    [SerializeField]
    private UpgradeObject boost;
    [SerializeField]
    private GameObject windColumn;

    protected override void TutorialMethod()
    {
        CheckIfHasBoost();
    }

    private void CheckIfHasBoost()
    {
        if (!UpgradeManager.Instance.CheckObtainedUpgrade(boost))
            return;

        StartBoostTutorial();
    }

    private void StartBoostTutorial()
    {
        boostCanvasTutorial.gameObject.SetActive(true);
        windColumn.SetActive(true);

        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();

        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);

        MenuControlsHint.Instance.UpdateHintControls(neededControls, null, MenuControlsHint.HintsPos.BOTTOM_LEFT);
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        boostCanvasTutorial.enabled = false;
        MenuControlsHint.Instance.UpdateHintControls(null);
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        boostCanvasTutorial.gameObject.SetActive(true);

        Destroy(this);
    }
}
