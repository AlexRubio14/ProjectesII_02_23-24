using UnityEngine;

public class ShopTutorial : Tutorial
{
    [Space, SerializeField]
    private GameObject shopTutorial;
    [SerializeField]
    private GameObject inventory;


    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(tutorialkey) && PlayerPrefs.GetInt(tutorialkey) == 1)
        {
            Destroy(this);
            return;
        }
        else
        {
            TutorialMethod();
        }
    }

    protected override void TutorialMethod()
    {
        dialogueController.dialogues = dialogues;

        dialogueController.onDialogueLineStart += DisplayShopTutorial;
        dialogueController.onDialogueEnd += EndTutorial;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();

    }

    private void DisplayShopTutorial(int _dialogueID)
    {
        if (_dialogueID != 1)
            return;

        shopTutorial.SetActive(true);
        inventory.transform.SetParent(shopTutorial.transform);
        inventory.transform.SetAsFirstSibling();
    }

    protected override void EndTutorial()
    {
        inventory.transform.SetParent(transform.GetChild(0));

        shopTutorial.SetActive(false);
        dialogueController.onDialogueLineStart -= DisplayShopTutorial;
        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);
        Awake();
    }
}
