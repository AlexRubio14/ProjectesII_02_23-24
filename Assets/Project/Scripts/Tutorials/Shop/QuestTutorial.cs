using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTutorial : Tutorial
{

    [SerializeField]
    private GameObject questTutorialCanvas;
    [SerializeField]
    private GameObject allQuestTutorialCanvas;
    [SerializeField]
    private GameObject requirementsTutorial;
    [SerializeField]
    private GameObject rewardsTutorial;
    [SerializeField]
    private GameObject selectButtonTutorial;

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
        questTutorialCanvas.SetActive(true);

        dialogueController.dialogues = dialogues;

        dialogueController.onDialogueLineStart += OnDialogueLineStart;
        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();
    }

    private void OnDialogueLineStart(int _dialogueLineID)
    {
        switch (_dialogueLineID)
        {
            case 0:
                //Tutorial de todo
                AllTutorial();
                break;
            case 1:
                //Tutorial de Requisitos
                RequirementsTutorial();
                break;
            case 3:
                //Tutorial de recompensas
                RewardsTutorial();
                break;
            case 4:
                //Tutorial de seleccionar
                SelectionTutorial();
                break;
            default:
                break;
        }
    }

    private void AllTutorial()
    {
        allQuestTutorialCanvas.SetActive(true);
    }

    private void RequirementsTutorial()
    {
        allQuestTutorialCanvas.SetActive(false);

        requirementsTutorial.SetActive(true);

    }

    private void RewardsTutorial()
    {
        requirementsTutorial.SetActive(false);

        rewardsTutorial.SetActive(true);
    }

    private void SelectionTutorial()
    {
        rewardsTutorial.SetActive(false);

        selectButtonTutorial.SetActive(true);
    }

    protected override void EndTutorial()
    {
        questTutorialCanvas.SetActive(false);

        dialogueController.onDialogueLineStart -= OnDialogueLineStart;
        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);
    }
}
