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
    private float cameraSizeBossFight;
    private float baseCamSize;
    [SerializeField]
    private PickableItemController endFightItem;


    [HideInInspector]
    public BossDoors door;


    private Camera cam;

    protected override void Awake()
    {
        foreach (MonoBehaviour item in bossScripts)
            item.enabled = false;

        bossCanvas.SetActive(false);
        
        endFightItem.onItemPicked += OnFightEnd;
    }

    private void Start()
    {
        cam = CameraController.Instance.GetComponent<Camera>();
        baseCamSize = cam.orthographicSize;
    }

    protected override void TutorialMethod()
    {

        CameraController.Instance.objectToFollow = cameraFollowTarget;
        cam.orthographicSize = cameraSizeBossFight;


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

        //Volver a la posicion de la puerta
        PlayerManager.Instance.player.transform.position = door.transform.position;
        door.DestroyDoor();

        cam.orthographicSize = baseCamSize;
        CameraController.Instance.objectToFollow = PlayerManager.Instance.player.gameObject;

    }

    private void OnDisable()
    {
        endFightItem.onItemPicked -= OnFightEnd;
    }
}
