using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuestController : MonoBehaviour
{
    [SerializeField]
    private Button firstButtonSelected;
    [Space, SerializeField]
    private GameObject shopButtons;

    [Space, Header("First Time Quest"), SerializeField]
    private QuestInfoMenu firstTimeQuest;
    [SerializeField]
    private Button firstTimeSelectedButton; 

    [Space, Header("Check List"), SerializeField]
    private CheckQuestController checkController;
    [SerializeField]
    private Button checkListSelectedButton;
    [HideInInspector]
    public List<QuestObject> newQuests;
    [HideInInspector]
    public bool checkDisplayNewQuests = false;
    [SerializeField]
    private Button questBackButton;

    [Space, SerializeField]
    private DialogueController dialogue;

    private void Start()
    {
        newQuests = new List<QuestObject>();

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


        if (checkController.isActiveAndEnabled && newQuests.Count <= 1)
            firstTimeQuest.onFirstQuestClosed[1] = checkController.questBackButton;
        

        if (firstTimeQuest.gameObject.activeInHierarchy)
            StartCoroutine(WaitSelectFirstTimeButton());

        

    }

    private void Update()
    {
        CheckForNewQuests();
    }
    
    private void CheckForNewQuests()
    {
        if (checkDisplayNewQuests && !firstTimeQuest.gameObject.activeInHierarchy)
        {
            if (newQuests.Count <= 0)
            {
                checkDisplayNewQuests = false;
                questBackButton.onClick.Invoke();
                return;
            }
            else if (newQuests[0].obtainedQuest)
            {
                newQuests.RemoveAt(0);
                return;
            }

            firstTimeQuest.gameObject.SetActive(true);
            firstTimeQuest.SetValues(newQuests[0]);
            shopButtons.SetActive(false);
        }
    }

    public void DisplayQuestList()
    {
        checkController.gameObject.SetActive(true);
        checkController.DisplayQuestCardList();
        checkListSelectedButton.Select();
    }



}
