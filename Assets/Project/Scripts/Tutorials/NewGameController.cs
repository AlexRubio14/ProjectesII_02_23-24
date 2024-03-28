using UnityEngine;
using UnityEngine.UI;

public class NewGameController : MonoBehaviour
{
    [SerializeField]
    private QuestObject firstQuest;

    private void Start()
    {
        Button continueButton = GetComponent<Button>();
        if (!firstQuest.obtainedQuest)
        {
            continueButton.interactable = false;
        }
        else
        {
            continueButton.Select();
        }
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
