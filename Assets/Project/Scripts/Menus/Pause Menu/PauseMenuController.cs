using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Inputs"), SerializeField]
    private InputActionReference pauseAction;
    [SerializeField]
    private InputActionReference resumeAction;
    [SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;

    [Space, Header("Pause Menu"), SerializeField]
    private Canvas pauseMenuCanvas;
    [SerializeField]
    private Button continueButton;

    [Space, Header("Quest List"), SerializeField]
    private Transform cardLayout;
    [SerializeField]
    private GameObject cardPrefab;
    private List<PauseMenuQuestCard> cardList;

    [Space, Header("Quest"), SerializeField]
    private TextMeshProUGUI titleQuestText;
    [SerializeField]
    private TextMeshProUGUI questStateText;
    [Header("Requirements"), SerializeField]
    private LayoutGroup requirementsLayout;
    private List<Image> requirementImages;
    private List<TextMeshProUGUI> requirementTexts;
    [Header("Rewards"), SerializeField]
    private LayoutGroup rewardLayout;
    private List<TextMeshProUGUI> rewardList;
    [SerializeField]
    private float rewardFontSize;
    [SerializeField]
    private TMP_FontAsset fontAsset;
    private QuestCanvasController questIngameCanvas;

    private void Awake()
    {
        questIngameCanvas = FindObjectOfType<QuestCanvasController>();
    }

    private void Start()
    {
        cardList = new List<PauseMenuQuestCard>();
        requirementImages = new List<Image>();
        requirementTexts = new List<TextMeshProUGUI>();
        rewardList = new List<TextMeshProUGUI>();


        DisplayQuestsObtainedList();
        DisplayFirstQuest();
    }

    private void OnEnable()
    {
        pauseAction.action.started += PauseGame;
        resumeAction.action.started += ResumeGame;
        InputSystem.onDeviceChange += UpdateInputImages;
        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);
    }
    private void OnDisable()
    {
        pauseAction.action.started -= PauseGame;
        resumeAction.action.started -= ResumeGame;
        InputSystem.onDeviceChange -= UpdateInputImages;

    }

    private void DisplayQuestsObtainedList()
    {
        List<QuestObject> obtainedQuests = QuestManager.Instance.GetAllObtainedQuests();

        //Crear miniaturas de las quests
        //Estas cambiaran dependiendo de si estan completadas o no
        foreach (QuestObject item in obtainedQuests)
        {
            GameObject currentObject = Instantiate(cardPrefab, cardLayout);
            PauseMenuQuestCard currentCard = currentObject.GetComponent<PauseMenuQuestCard>();
            currentCard.SetupQuest(item);
            cardList.Add(currentCard);
        }
    }
    private void DisplayFirstQuest()
    {
        if (cardList.Count == 0)
            return;

        QuestObject selectedQuest = QuestManager.Instance.GetSelectedQuest();

        if (!selectedQuest)
            selectedQuest = QuestManager.Instance.GetAllObtainedQuests()[0];


        DisplayQuest(selectedQuest);

    }
    public void SelectQuest(PauseMenuQuestCard _selectedCard)
    {
        if (_selectedCard.currentQuest.completedQuest)
            return;

        QuestObject lastSelectedQuest = QuestManager.Instance.GetSelectedQuest();
        QuestManager.Instance.SetSelectedQuest(_selectedCard.currentQuest.questID);
        _selectedCard.SetupQuest(_selectedCard.currentQuest);

        foreach (PauseMenuQuestCard item in cardList)
        {
            if (item.currentQuest == lastSelectedQuest)
            {
                item.SetupQuest(lastSelectedQuest);
                break;
            }
        }

        questIngameCanvas.RemoveCurrentQuest();
        questIngameCanvas.SetupQuestCanvas(_selectedCard.currentQuest);
    }

    #region Quest
    public void DisplayQuest(QuestObject _selectedQuest)
    {
        titleQuestText.text = _selectedQuest.questTitle.ToString() + ": " + _selectedQuest.questName.ToString();
        if (_selectedQuest.completedQuest)
            questStateText.text = "Completada";
        else
            questStateText.text = "";

        SetRequirementValues(_selectedQuest);
        SetRewardValues(_selectedQuest);

        _selectedQuest.newQuest = false;
    }
    private void SetRequirementValues(QuestObject _selectedQuest)
    {
        Dictionary<ItemObject, short> inventory = InventoryManager.Instance.GetAllItems();

        //Si no hay suficientes objetos crea nuevos
        if (inventory.Count > requirementTexts.Count)
        {
            for (int i = requirementTexts.Count; i < inventory.Count; i++)
            {
                GameObject newObj = new GameObject("Requirement Image");
                Image newImage = newObj.AddComponent<Image>();
                requirementImages.Add(newImage);
                newObj.transform.SetParent(requirementsLayout.transform);
                newObj.transform.localScale = Vector3.one;

                newObj = new GameObject("Requirement Text");
                TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
                newText.font = fontAsset;
                newText.enableAutoSizing = true;
                newText.horizontalAlignment = HorizontalAlignmentOptions.Center;
                newText.verticalAlignment = VerticalAlignmentOptions.Middle;
                newText.enableWordWrapping = false;
                requirementTexts.Add(newText);

                newObj.transform.SetParent(requirementsLayout.transform);
                newObj.transform.localScale = Vector3.one;
            }
        }

        //Asignarle los valores que le toquen segun la quest que estas haciendo
        int loopIndex = 0;
        foreach (KeyValuePair<ItemObject, short> item in _selectedQuest.neededItems)
        {
            requirementImages[loopIndex].gameObject.SetActive(true);
            requirementImages[loopIndex].sprite = item.Key.PickableSprite;

            requirementTexts[loopIndex].gameObject.SetActive(true);
            requirementTexts[loopIndex].text = inventory[item.Key] + " / " + item.Value;

            loopIndex++;
        }

        //Desactivar las imagenes y textos que sobren
        if (loopIndex < requirementTexts.Count)
        {
            for (int i = loopIndex; i < requirementTexts.Count; i++)
            {
                requirementImages[i].gameObject.SetActive(false);
                requirementTexts[i].gameObject.SetActive(false);
            }
        }

    }
    private void SetRewardValues(QuestObject _selectedQuest)
    {
        //Si no hay suficientes objetos crea nuevos
        if (_selectedQuest.rewards.Count > rewardList.Count)
        {
            for (int i = rewardList.Count; i < _selectedQuest.rewards.Count; i++)
            {
                GameObject newObj = new GameObject("Reward Text");
                TextMeshProUGUI newText = newObj.AddComponent<TextMeshProUGUI>();
                newText.fontSize = rewardFontSize;
                newText.font = fontAsset;
                newObj.transform.SetParent(rewardLayout.transform);

                rewardList.Add(newText);
                newObj.transform.localScale = Vector3.one;

            }
        }


        //Asignarle los valores que le toquen segun la quest que estas haciendo
        int loopIndex = 0;
        foreach (KeyValuePair<ScriptableObject, QuestObject.RewardType> item in _selectedQuest.rewards)
        {
            if (item.Value == QuestObject.RewardType.NEW_QUEST)
            {
                rewardList[loopIndex].gameObject.SetActive(false);
                loopIndex++;
                continue;
            }


            rewardList[loopIndex].gameObject.SetActive(true);

            switch (item.Value)
            {
                case QuestObject.RewardType.UPGRADE:
                    rewardList[loopIndex].text = ((UpgradeObject)item.Key).UpgradeName + ": " + ((UpgradeObject)item.Key).UpgradeDescription;
                    break;
                case QuestObject.RewardType.POWER_UP:
                    rewardList[loopIndex].text = ((ItemObject)item.Key).ItemName;
                    break;
                default:
                    break;
            }

            loopIndex++;
        }

        //Desactivar los textos que sobren
        if (loopIndex < rewardList.Count)
        {
            for (int i = loopIndex; i < rewardList.Count; i++)
            {
                rewardList[i].gameObject.SetActive(false);
            }
        }

    }
    #endregion


    private void PauseGame(InputAction.CallbackContext obj)
    {
        InputController.Instance.ChangeActionMap("Menu");
        TimeManager.Instance.PauseGame();
        pauseMenuCanvas.gameObject.SetActive(true);

        SelectFirstQuestButon();
    }
    public void SelectFirstQuestButon()
    {
        QuestObject selectedQuest = QuestManager.Instance.GetSelectedQuest();

        if (!selectedQuest)
        {
            continueButton.Select();
            return;
        }

        foreach (PauseMenuQuestCard card in cardList)
        {
            card.SetupQuest(card.currentQuest);

            if (card.currentQuest == selectedQuest)
            {
                card.GetComponent<Button>().Select();
                DisplayQuest(card.currentQuest);
            }

        }


        
    }

    public void ResumeGameButton()
    {
        ResumeGame(new InputAction.CallbackContext());
    }

    private void ResumeGame(InputAction.CallbackContext obj)
    {
        InputController.Instance.ChangeActionMap("Player");
        TimeManager.Instance.ResumeGame();
        pauseMenuCanvas.gameObject.SetActive(false);
    }

    public void AbortMission()
    {
        PlayerManager.Instance.player.SubstractFuel(1000);
        PlayerManager.Instance.player.fuelConsume = -10;
        ResumeGame(new InputAction.CallbackContext());
    }

    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }
}


