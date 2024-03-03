using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossApologize : Tutorial
{
    [SerializeField]
    private GameObject apologizeReward;
    [SerializeField]
    private GameObject catMonster;

    protected override void TutorialMethod()
    {
        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();


        catMonster.SetActive(true);

        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);

        MenuControlsHint.Instance.UpdateHintControls(neededControls, null, MenuControlsHint.HintsPos.BOTTOM_RIGHT);
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        MenuControlsHint.Instance.UpdateHintControls(null);
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        //Instanciar activar objeto recogible
        apologizeReward.SetActive(true);
        catMonster.SetActive(false);

        PlayerPrefs.SetInt(tutorialkey, 1);

        Destroy(this);
    }
}
