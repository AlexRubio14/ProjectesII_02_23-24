using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuestController : MonoBehaviour
{
    [SerializeField]
    private Button firstButtonSelected;

    [Space, Header("First Time Quest"), SerializeField]
    private QuestInfoMenu firstTimeQuest;
    [SerializeField]
    private Button firstTimeSelectedButton; 

    [Space, Header("Check List"), SerializeField]
    private CheckQuestController checkController;
    [SerializeField]
    private Button checkListSelectedButton;
    [HideInInspector]
    public bool checkDisplayNewQuests = false;
    [SerializeField]
    private Button questBackButton;

    [Space, SerializeField]
    private DialogueController dialogue;

    private void Start()
    {

        //Esto solo funcionara si acabamos de empezar el juego, nos seleccionara la primera quest de todas y nos saltara el 
        QuestObject selectedQuest = QuestManager.Instance.GetSelectedQuest();
        if (selectedQuest && !selectedQuest.obtainedQuest)
        {
            firstTimeQuest.gameObject.SetActive(true);
            firstTimeQuest.SetValues(selectedQuest);
        }
        else
        {
            firstButtonSelected.Select();
        }
    }

    private void OnEnable()
    {
        dialogue.onDialogueEnd += OnDialogueEnd;
    }

    private void OnDisable()
    {
        dialogue.onDialogueEnd -= OnDialogueEnd;
    }
    private void OnDialogueEnd()
    {
        IEnumerator WaitSelectFirstTimeButton()
        {
            yield return new WaitForEndOfFrame();
            firstTimeSelectedButton.Select();
        }


        if (checkController.isActiveAndEnabled)
            firstTimeQuest.onFirstQuestClosed[1] = checkController.questBackButton;
        

        if (firstTimeQuest.gameObject.activeInHierarchy)
            StartCoroutine(WaitSelectFirstTimeButton()); 
    }
}
