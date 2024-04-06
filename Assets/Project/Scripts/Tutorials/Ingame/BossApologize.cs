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
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        //Instanciar activar objeto recogible
        apologizeReward.SetActive(true);
        catMonster.SetActive(false);

        PlayerPrefs.SetInt(tutorialkey, 1);

        Destroy(this);
    }
}
