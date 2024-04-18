using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CheckQuestController : MonoBehaviour
{
    private QuestObject currentQuest;


    
    [Header("Card List"), SerializeField]
    private GameObject questListUi;
    [SerializeField]
    private Transform cardLayout;
    [SerializeField]
    private Button cardListBackButton;
    public Button questBackButton;
    [SerializeField]
    private GameObject cardPrefab;
    private List<QuestCard> cardList;   


    [Space, Header("Quest"), SerializeField]
    private QuestInfoMenu questCanvas;
    [SerializeField]
    private Button completeButton;
    private TextMeshProUGUI completeText;
    [SerializeField]
    private Button selectButton;
    [SerializeField]
    private Button backButton;
    private TextMeshProUGUI selectText;
    [SerializeField]
    private DialogueController dialogueController;
    [SerializeField]
    private UpgradeInstructionController upgradeInstruction;
    [SerializeField]
    private Button upgradeInstructionObtainButton;
    [SerializeField]
    private AudioClip missionCompletedClip;

    [Space, Header("Inventory"), SerializeField]
    private InventoryMenuController inventoryMenu;


    private void Awake()
    {
        cardList = new List<QuestCard>();

        completeText = completeButton.GetComponentInChildren<TextMeshProUGUI>();
        selectText = selectButton.GetComponentInChildren<TextMeshProUGUI>();


        completeButton.onClick.AddListener(() => CompleteQuest());
        selectButton.onClick.AddListener(() => SelectQuest());
    }

    public void UpdateQuestValues(QuestObject _quest, bool _selectButton = true)
    {
        if (_selectButton)
        {
            backButton.Select();
        }

        questListUi.SetActive(false);

        currentQuest = _quest;
        questCanvas.gameObject.SetActive(true);
        questCanvas.SetValues(currentQuest);

        //Inicializar botones
        if (!currentQuest.completedQuest)
        {
            bool canBuy = InventoryManager.Instance.CanBuy(_quest.neededItems);
            completeButton.interactable = canBuy;
            if (canBuy)
                completeButton.Select();
            //completeButtonFloatEffect.canFloat = canBuy;
            completeText.text = "To Complete";


            selectButton.gameObject.SetActive(true);

            if (_quest == QuestManager.Instance.GetSelectedQuest())
            {
                selectButton.interactable = false;
                selectText.text = "Fijada";
            }
            else
            {
                selectButton.interactable = true;
                selectText.text = "Fijar";
            }
        }
        else
        {
            completeButton.interactable = false;
            completeText.text = "Completed";

            selectButton.gameObject.SetActive(false);
        }

        if (currentQuest.newQuest)
            currentQuest.newQuest = false;

        QuestManager.Instance.SaveQuests();

        //Settear que los items que necesita la quest hagan el efecto de flotar
        foreach (KeyValuePair<ItemObject, short> item in _quest.neededItems)
        {
            inventoryMenu.SetItemFloaty(item.Key, true);
        }
    }
    private void CompleteQuest()
    {
        InventoryManager.Instance.Buy(currentQuest.neededItems);
        currentQuest.completedQuest = true;

        QuestManager.Instance.SaveQuests();

        foreach (KeyValuePair<ScriptableObject, QuestObject.RewardType> item in currentQuest.rewards)
        {
            switch (item.Value)
            {
                case QuestObject.RewardType.UPGRADE:
                    UpgradeObject currentUpgrade = (UpgradeObject)item.Key; 
                    UpgradeManager.Instance.ObtainUpgrade(currentUpgrade);

                    IEnumerator DisplayUpgradeInstructions()
                    {
                        yield return new WaitForEndOfFrame();

                        upgradeInstruction.gameObject.SetActive(true);
                        upgradeInstruction.SetUpgradeInstructions(currentUpgrade);

                        upgradeInstructionObtainButton.Select(); 
                    }
                    StartCoroutine(DisplayUpgradeInstructions());
                    break;
                case QuestObject.RewardType.NEW_QUEST:
                    ((QuestObject)item.Key).obtainedQuest = true;
                    break;
                case QuestObject.RewardType.POWER_UP:
                    ItemObject currentItem = (ItemObject)item.Key;
                    PowerUpManager.Instance.PowerUpObtained(currentItem.PowerUp);
                    break;
                default:
                    break;
            }
        }

        RemoveQuestCardList();
        questCanvas.RemoveQuestInfo();
        UpdateQuestValues(currentQuest, false);

        foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
        {
            inventoryMenu.UpdateItemAmount(item.Key);
        }

        dialogueController.dialogues = currentQuest.questDialogueEnd;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();

        AudioManager.instance.Play2dOneShotSound(missionCompletedClip, "MissionCompleted", 1, 1, 1);
    }

    private void SelectQuest()
    {
        QuestManager.Instance.SetSelectedQuest(currentQuest.questID);
        questCanvas.RemoveQuestInfo();
        UpdateQuestValues(currentQuest);
    }

    public void DisplayQuestCardList()
    {
        if (!gameObject.activeInHierarchy)
            return;

        questListUi.SetActive(true);

        if (currentQuest)
        {
            foreach (KeyValuePair<ItemObject, short> item in currentQuest.neededItems)
            {
                inventoryMenu.SetItemFloaty(item.Key, false);
            }
        }
        
        List<QuestObject> obtainedQuests = QuestManager.Instance.GetAllObtainedQuests();

        //Crear miniaturas de las quests
        //Estas cambiaran dependiendo de si estan completadas o no
        foreach (QuestObject item in obtainedQuests)
        {
            GameObject currentObject = Instantiate(cardPrefab, cardLayout);
            QuestCard currentCard = currentObject.GetComponent<QuestCard>();
            cardList.Add(currentCard);
            currentCard.SetTextValues(item);
        }

        SelectButtonQuestList();
    }

    private void SelectButtonQuestList()
    {
        QuestObject questObject;

        if (currentQuest)
        {
            questObject = currentQuest;
        }
        else if(QuestManager.Instance.GetSelectedQuest())
        {
            questObject = QuestManager.Instance.GetSelectedQuest();
        }
        else
        {
            questObject = cardList[0].currentQuest;
        }

        for(int i = 0; i  < cardList.Count; i++) 
        {
            if(questObject == cardList[i].currentQuest)
            {
                cardList[i].GetComponent<Button>().Select();
                return;
            }
        }
    }
    public void RemoveQuestCardList()
    {
        foreach (QuestCard item in cardList)
        {
            Destroy(item.gameObject);
        }

        cardList.Clear();   
    }
}
