using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private Image questStateBackground;
    [SerializeField]
    private TextMeshProUGUI questStateText;
    [SerializeField]
    private Color completeColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color newQuestColor;

    [SerializeField]
    private Color completed;

    [HideInInspector]
    public QuestObject currentQuest;

    private Button checkCardButton;
    private Image currentImage; 

    private void Awake()
    {
        currentImage = GetComponent<Image>();
        checkCardButton = GetComponent<Button>();
    }

    public void SetTextValues(QuestObject _quest)
    {
        currentQuest = _quest;
        titleText.text = currentQuest.questTitle;
        nameText.text = currentQuest.questName;

        CheckQuestController checkQuestCont = GetComponentInParent<CheckQuestController>();

        checkCardButton.onClick.AddListener(() => checkQuestCont.UpdateQuestValues(currentQuest));

        if (currentQuest.completedQuest)
        {
            //Completada
            currentImage.color = completed; 
            questStateBackground.color = completeColor;
            questStateText.text = "Completed";
        }
        else if (QuestManager.Instance.GetSelectedQuest() == currentQuest)
        {
            //Seleccionada
            questStateBackground.color = selectedColor;
            questStateText.text = "Selected";
        }
        else if (currentQuest.newQuest)
        {
            //Mision nueva
            questStateBackground.color = newQuestColor;
            questStateText.text = "New";
        }
        else
        {
            //Desactivarlo
            questStateBackground.gameObject.SetActive(false);
        }

    }

}
