using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTutorial : Tutorial
{
    [SerializeField]
    private Canvas boostCanvasTutorial;

    [SerializeField]
    private UpgradeObject boost;

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

        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        boostCanvasTutorial.enabled = false;
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        boostCanvasTutorial.gameObject.SetActive(true);

        Destroy(this);
    }
}
