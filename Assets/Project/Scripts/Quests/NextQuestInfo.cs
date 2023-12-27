using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NextQuestInfo : MonoBehaviour
{
    [SerializeField]
    private Button firstSelectedButton;

    [SerializeField]
    private TextMeshProUGUI nextQuestText;

    private void OnEnable()
    {
        firstSelectedButton.Select();

        SetNextQuestText(QuestManager.Instance.GetCurrentQuest());
    }

    public void SetNextQuestText(QuestObject _nextQuest)
    {
        string nextQuestString;
        if (_nextQuest)
        {
            //nextQuestString = "Nuestra siguiente operacion tiene el nombre en clave de '" + _nextQuest.questName + "' \n"  + _nextQuest.questIntroduction;
        }
        else
        {
            nextQuestString = "Felicidades, ya no tengo mas misiones que darte *clap* *clap* *clap* \n Ahora ya tienes todas las mejoras desbqueadas, no te olvides pasarte por la tienda a echarles un ojo";
        }
        //nextQuestText.text = nextQuestString;
    }
}
