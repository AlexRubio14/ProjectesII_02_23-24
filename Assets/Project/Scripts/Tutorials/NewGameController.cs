using UnityEngine;
using UnityEngine.UI;

public class NewGameController : MonoBehaviour
{
    [SerializeField]
    private QuestObject firstQuest;
    [SerializeField]
    protected string firstTutorialkey;

    private void Start()
    {
        Button continueButton = GetComponent<Button>();

        if (PlayerPrefs.HasKey(firstTutorialkey) && PlayerPrefs.GetInt(firstTutorialkey) == 1)
            continueButton.Select();
        else
            continueButton.interactable = false;

    }

    public void ResetGame()
    {
        SelectTpsManager.instance.ResetTpObjects();

        PlayerPrefs.DeleteAll();
        QuestObject[] quests = Resources.LoadAll<QuestObject>("Quests");
        foreach (QuestObject item in quests)
        {
            item.obtainedQuest = false;
            item.completedQuest = false;
            item.newQuest = true;
        }

        InventoryManager.Instance.ResetInventory();

    }

}
