using UnityEngine;
using UnityEngine.UI;

public class NewGameController : MonoBehaviour
{
    [SerializeField]
    private QuestObject firstQuest;
    [SerializeField]
    protected string firstTutorialkey;

    [SerializeField]
    private MenuNavegation menuNavegation;

    [SerializeField]
    private AudioMenuController audioMenuController;
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

        QuestManager.Instance.ResetQuests();
        InventoryManager.Instance.ResetInventory();
        UpgradeManager.Instance.ResetUpgrades();
        PowerUpManager.Instance.ResetPowerUps();
    }

    private void GoToHub()
    {
        menuNavegation.GoToHub();
        TransitionCanvasManager.instance.onFadeIn -= GoToHub;

    }

    public void SubscribeToFadeIn()
    {
        TransitionCanvasManager.instance.onFadeIn += GoToHub;
        TransitionCanvasManager.instance.FadeIn();
    }

}
