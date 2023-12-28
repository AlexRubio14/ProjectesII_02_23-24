using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckQuestController : MonoBehaviour
{
    private QuestObject currentQuest;

    private DisplayQuestController displayQuestController;

    [Header("Card List"), SerializeField]
    private Transform cardLayout;
    [SerializeField]
    private GameObject cardPrefab;
    private List<GameObject> cardList;   


    [Space, Header("Quest"), SerializeField]
    private QuestInfoMenu questCanvas;
    [SerializeField]
    private Button completeButton;
    private TextMeshProUGUI completeText;
    [SerializeField]
    private Button selectButton;
    private TextMeshProUGUI selectText;

    private void Awake()
    {
        cardList = new List<GameObject>();

        displayQuestController = GetComponentInParent<DisplayQuestController>();
        completeText = completeButton.GetComponentInChildren<TextMeshProUGUI>();
        selectText = selectButton.GetComponentInChildren<TextMeshProUGUI>();

        completeButton.onClick.AddListener(() => CompleteQuest());
        selectButton.onClick.AddListener(() => SelectQuest());
    }

    public void UpdateQuestValues(QuestObject _quest)
    {
        currentQuest = _quest;
        questCanvas.gameObject.SetActive(true);
        questCanvas.SetValues(currentQuest);

        //Inicializar botones
        if (!currentQuest.completedQuest)
        {
            completeButton.interactable = InventoryManager.Instance.CanBuy(_quest.neededItems);
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
                    displayQuestController.checkDisplayNewQuests = true;
                    displayQuestController.newQuests.Add((QuestObject)item.Key);
                    break;
                default:
                    break;
            }
        }

        RemoveQuestCardList();
        questCanvas.RemoveQuestInfo();
        UpdateQuestValues(currentQuest);
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
