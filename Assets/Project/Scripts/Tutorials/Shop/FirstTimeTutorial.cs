using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirstTimeTutorial : Tutorial
{
    [Space, Header("Tutorial"), SerializeField]
    private QuestInfoMenu quest;

    [Space, SerializeField]
    private GameObject firstTimeTutorial;
    [SerializeField]
    private GameObject shipTutorial;
    [SerializeField]
    private GameObject[] questTutorial;
    [SerializeField]
    private GameObject questInfoParent;
    [SerializeField]
    private GameObject[] questInfo;


    [Space, SerializeField]
    private GameObject shopButton;
    [SerializeField]
    private GameObject backButton;

    [SerializeField]
    private GameObject requirementsLayout;


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
        shopButton.SetActive(false);
        backButton.SetActive(false);
        firstTimeTutorial.SetActive(true);
        dialogueController.onDialogueLineStart += DisplayRequisitesTutorial;
        dialogueController.onDialogueEnd += EndTutorial;
        quest.canItemsFloat = false;
    }

    protected override void EndTutorial()
    {
        for (int i = 0; i < requirementsLayout.transform.childCount; i++)
        {
            ImageFloatEffect floatFX = requirementsLayout.transform.GetChild(i).GetComponent<ImageFloatEffect>();
            if (floatFX)
            {
                floatFX.canFloat = true;
            }
        }

        foreach (GameObject item in questInfo)
        {
            item.transform.SetParent(questInfoParent.transform);
        }

        dialogueController.onDialogueLineStart -= DisplayRequisitesTutorial;
        dialogueController.onDialogueEnd -= EndTutorial;
        firstTimeTutorial.SetActive(false);
        PlayerPrefs.SetInt(tutorialkey, 1);
        Awake();
    }


    private void DisplayRequisitesTutorial(int _dialogueLineID)
    {
        switch (_dialogueLineID)
        {
            case 1:
                shipTutorial.SetActive(true);
                break; 
            case 2:
                shipTutorial.SetActive(false);

                for (int i = 0; i < questTutorial.Length; i++)
                {
                    questTutorial[i].SetActive(true);
                    questInfo[i].transform.SetParent(questTutorial[i].transform);
                    questInfo[i].transform.SetAsFirstSibling();
                }

                break; 
            default:
                break;
        }
    }

}
