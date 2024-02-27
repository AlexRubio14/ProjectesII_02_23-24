using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CheckQuestController : MonoBehaviour
{
    private QuestObject currentQuest;

    private DisplayQuestController displayQuestController;

    
    [Header("Card List"), SerializeField]
    private GameObject questListUi;
    [SerializeField]
    private Transform cardLayout;
    [SerializeField]
    private Button cardListBackButton;
    public Button questBackButton;
    [SerializeField]
    private GameObject cardPrefab;
    private List<GameObject> cardList;   


    [Space, Header("Quest"), SerializeField]
    private QuestInfoMenu questCanvas;
    [SerializeField]
    private Button completeButton;
    private TextMeshProUGUI completeText;
    [SerializeField]
    private ImageFloatEffect completeButtonFloatEffect;
    [SerializeField]
    private Button selectButton;
    [SerializeField]
    private Button backButton;
    private TextMeshProUGUI selectText;
    private DialogueController dialogueController;

    [Space, Header("Inventory"), SerializeField]
    private InventoryMenuController inventoryMenu;

    private void Awake()
    {
        cardList = new List<GameObject>();

        displayQuestController = GetComponentInParent<DisplayQuestController>();
        completeText = completeButton.GetComponentInChildren<TextMeshProUGUI>();
        completeButtonFloatEffect = completeButton.GetComponent<ImageFloatEffect>();
        selectText = selectButton.GetComponentInChildren<TextMeshProUGUI>();


        completeButton.onClick.AddListener(() => CompleteQuest());
        selectButton.onClick.AddListener(() => SelectQuest());
    }

    public void UpdateQuestValues(QuestObject _quest, bool _selectButton = true)
    {
        if (_selectButton)
        {
            cardLayout.gameObject.SetActive(false);
            cardListBackButton.gameObject.SetActive(false);
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
            //completeButtonFloatEffect.canFloat = canBuy;
            completeText.text = "Completar";


            selectButton.gameObject.SetActive(true);

            if (_quest == QuestManager.Instance.GetSelectedQuest())
            {
                selectButton.interactable = false;
                selectText.text = "Seleccionado";
            }
            else
            {
                selectButton.interactable = true;
                selectText.text = "Seleccionar";
            }
        }
        else
        {
            completeButton.interactable = false;
            completeText.text = "Completado";

            selectButton.gameObject.SetActive(false);
        }

        if (currentQuest.newQuest)
            currentQuest.newQuest = false;



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

        foreach (KeyValuePair<ScriptableObject, QuestObject.RewardType> item in currentQuest.rewards)
        {
            switch (item.Value)
            {
                case QuestObject.RewardType.UPGRADE:
                    UpgradeManager.Instance.ObtainUpgrade((UpgradeObject)item.Key);
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
            currentObject.GetComponent<QuestCard>().SetTextValues(item);
            cardList.Add(currentObject);
        }
    }
    public void RemoveQuestCardList()
    {
        foreach (GameObject item in cardList)
        {
            Destroy(item);
        }
    }
}
