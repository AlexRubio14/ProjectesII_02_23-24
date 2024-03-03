using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuQuestCard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private Image questTagBackground;
    [SerializeField]
    private TextMeshProUGUI questTagText;

    [Space, SerializeField]
    private Color completeColorTag;
    [SerializeField]
    private Color selectedColorTag;
    [SerializeField]
    private Color newQuestColorTag;
    [SerializeField]
    private Color completedBackgroundColor;

    private Button checkCardButton;
    [SerializeField]
    private Image currentImage;

    private EventTrigger eventTrigger;
    private PauseMenuController pauseMenu;

    public QuestObject currentQuest {  get; private set; }

    private void Awake()
    {
        checkCardButton = GetComponent<Button>();
        eventTrigger = GetComponent<EventTrigger>();
        pauseMenu = GetComponentInParent<PauseMenuController>();
    }

    private void Start()
    {
        checkCardButton.onClick.AddListener(() => pauseMenu.SelectQuest(this));
        
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((eventData) => { pauseMenu.DisplayQuest(currentQuest); } );

        eventTrigger.triggers.Add(entry);
    }

    public void SetupQuest(QuestObject _currentQuest)
    {
        currentQuest = _currentQuest;
        titleText.text = currentQuest.questTitle;

        questTagBackground.gameObject.SetActive(true);

        if (currentQuest.completedQuest)
        {
            //Completada
            currentImage.color = completedBackgroundColor;
            questTagBackground.color = completeColorTag;
            questTagText.text = "Completada";
        }
        else if (QuestManager.Instance.GetSelectedQuest() == currentQuest)
        {
            //Seleccionada
            questTagBackground.color = selectedColorTag;
            questTagText.text = "Seleccionado";
        }
        else if (currentQuest.newQuest)
        {
            //Mision nueva
            questTagBackground.color = newQuestColorTag;
            questTagText.text = "Nuevo";
        }
        else
        {
            //Desactivarlo
            questTagBackground.gameObject.SetActive(false);
        }
    }

}
