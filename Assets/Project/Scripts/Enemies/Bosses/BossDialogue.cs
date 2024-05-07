using System.Diagnostics;
using UnityEngine;

public class BossDialogue : Tutorial
{

    [Space, SerializeField]
    private string dialogueString;

    [SerializeField]
    private MonoBehaviour[] bossScripts;
    [SerializeField]
    private GameObject bossCanvas;
    [Space, SerializeField]
    private GameObject cameraFollowTarget;
    [SerializeField]
    private PickableItemController endFightItem;


    [HideInInspector]
    public BossDoors door;

    [TextArea, SerializeField]
    public string[] endDialogues;

    protected override void Awake()
    {
        foreach (MonoBehaviour item in bossScripts)
            item.enabled = false;

        bossCanvas.SetActive(false);
        
        endFightItem.onItemPicked += OnFightEnd;
    }

    protected override void TutorialMethod()
    {

        BossManager.Instance.onBossEnter();
        CameraController.Instance.objectToFollow = cameraFollowTarget;

        if (!PlayerPrefs.HasKey(dialogueString))
        {
            dialogueController.onDialogueEnd += EndTutorial;

            dialogueController.dialogues = dialogues;
            dialogueController.gameObject.SetActive(true);

            dialogueController.StartDialogue();
            TimeManager.Instance.PauseGame();
        }
        else
            EndTutorial();
        
        
    }

    protected override void EndTutorial()
    {
        TimeManager.Instance.ResumeGame();

        foreach (MonoBehaviour item in bossScripts)
            item.enabled = true;

        bossCanvas.SetActive(true);

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(dialogueString, 1);

        endFightItem.onItemPicked += OnFightEnd;

    }

    private void OnFightEnd()
    {
        PlayerPrefs.SetInt(tutorialkey, 1);

        bossCanvas.SetActive(false);

        dialogueController.dialogues = endDialogues;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();

        dialogueController.onDialogueEnd += EndFight;
    }

    private void EndFight()
    {
        TransitionCanvasManager.instance.FadeIn();

        TransitionCanvasManager.instance.onFadeIn += GoToMap;

        dialogueController.onDialogueEnd -= EndFight;
    }

    private void GoToMap()
    {
        PlayerManager.Instance.player.transform.position = door.posToBack.position;
        CameraController.Instance.objectToFollow = PlayerManager.Instance.player.gameObject;

        BossManager.Instance.onBossExit();
        TransitionCanvasManager.instance.FadeOut();
    }
    private void OnDisable()
    {
        endFightItem.onItemPicked -= OnFightEnd;
    }
}
