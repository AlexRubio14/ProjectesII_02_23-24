using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ShopTutorial : Tutorial
{
    [Space, SerializeField]
    private GameObject shopTutorial;

    private void OnEnable()
    {
        TutorialMethod();
    }

    protected override void TutorialMethod()
    {
        dialogueController.dialogues = dialogues;

        dialogueController.onDialogueLineStart += DisplayShopTutorial;
        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();

    }

    private void DisplayShopTutorial(int _dialogueID)
    {
        if (_dialogueID != 1)
            return;

        shopTutorial.SetActive(true);

    }

    protected override void EndTutorial()
    {
        shopTutorial.SetActive(false);
        dialogueController.onDialogueLineStart -= DisplayShopTutorial;
        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);
        PlayerPrefs.Save();

        Awake();
    }
}
