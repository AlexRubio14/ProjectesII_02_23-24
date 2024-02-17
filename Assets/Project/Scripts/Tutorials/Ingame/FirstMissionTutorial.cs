using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMissionTutorial : MonoBehaviour
{


    [SerializeField]
    private QuestObject oresInFirstQuest;

    private bool tutorialHasShown;

    
    [Space, Header("Tutorial"), SerializeField]
    protected string tutorialkey;

    [TextArea, SerializeField]
    public string[] dialogues;

    [SerializeField]
    protected DialogueController dialogueController;

    private Dictionary<ItemObject, short> obtainedOres;

    private void Awake()
    {
        tutorialHasShown = false;
    }

    private void OnEnable()
    {
        InventoryManager.Instance.obtainItemAction += CheckIfFirstMissionIsCompleted;

        obtainedOres = new Dictionary<ItemObject, short>();

        foreach(KeyValuePair<ItemObject, short> pair in oresInFirstQuest.neededItems)
        {
            obtainedOres.Add(pair.Key, 0);
        }
    }

    private void OnDisable()
    {
        InventoryManager.Instance.obtainItemAction -= CheckIfFirstMissionIsCompleted;

    }

    private void StartFirstMissionTutorial()
    {
        tutorialHasShown = true;

        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();

        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);

        MenuControlsHint.Instance.UpdateHintControls(neededControls, null, MenuControlsHint.HintsPos.BOTTOM_RIGHT);
        TimeManager.Instance.PauseGame();
    }

    private void EndTutorial()
    {
        MenuControlsHint.Instance.UpdateHintControls(null);

        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        Destroy(this);
    }

    private void CheckIfFirstMissionIsCompleted(ItemObject type, short s)
    {
        if(obtainedOres.ContainsKey(type))
        {
            obtainedOres[type] = s;
            CheckIfEnoughItems();
        }
    }

    private void CheckIfEnoughItems()
    {
        if (tutorialHasShown)
            return;

        int acumulator = 0;

        foreach (KeyValuePair<ItemObject, short> pair in oresInFirstQuest.neededItems)
        {
            if (obtainedOres[pair.Key] >= oresInFirstQuest.neededItems[pair.Key])
            {
                acumulator++;
            }
        }

        if (acumulator >= obtainedOres.Count)
            StartFirstMissionTutorial();
    }
}
