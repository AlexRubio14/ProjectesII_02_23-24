using UnityEngine;

public class FirstCompletedMission : Tutorial
{

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(tutorialkey) && PlayerPrefs.GetInt(tutorialkey) == 1)
        {
            Destroy(this);
            return;
        }
        else
        {
            TutorialMethod();
        }
    }

    protected override void TutorialMethod()
    {
        dialogueController.dialogues = dialogues;

        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();
    }

    protected override void EndTutorial()
    {

        dialogueController.onDialogueEnd -= EndTutorial;    

        PlayerPrefs.SetInt(tutorialkey, 1);
    }
}
