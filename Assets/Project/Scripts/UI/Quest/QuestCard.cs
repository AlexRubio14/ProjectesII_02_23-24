using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI nameText;

    public QuestObject currentQuest;

    private Button checkCardButton;

    private void Awake()
    {
        checkCardButton = GetComponent<Button>();
    }

    public void SetTextValues(QuestObject _quest)
    {
        currentQuest = _quest;
        titleText.text = currentQuest.questTitle;
        nameText.text = currentQuest.questName;

        CheckQuestController checkQuestCont = GetComponentInParent<CheckQuestController>();

        checkCardButton.onClick.AddListener(() => checkQuestCont.UpdateQuestValues(currentQuest));
    }

}
