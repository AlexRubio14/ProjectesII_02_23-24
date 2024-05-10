using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    [TextArea, SerializeField] 
    public string[] dialogues;

    [SerializeField]
    private DialogueController dialogueController;

    [SerializeField]
    private List<GameObject> enemies; 

    [SerializeField]
    private CannonController cannonController;

    private bool allKilled;

    [SerializeField]
    private AudioClip audioClip;
    private AudioSource audioSource;

    private void Start()
    {
        allKilled = false;
    }

    private void Update()
    {
        AllEnemiesKilled(); 
    }

    private void OnEnable()
    {
        TransitionCanvasManager.instance.FadeOut();
        TransitionCanvasManager.instance.onFadeOut += StartDialogue;

        audioSource = AudioManager.instance.Play2dLoop(audioClip, "Music", 1.0f, 1.0f, 1.0f);
    }

    public void StartDialogue()
    {
        dialogueController.onDialogueEnd += EndDialogue;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);
        dialogueController.StartDialogue();

        TimeManager.Instance.PauseGame();

        TransitionCanvasManager.instance.onFadeOut -= StartDialogue;
    }
    public void EndDialogue()
    {
        TimeManager.Instance.ResumeGame();
        cannonController.enabled = true;
        dialogueController.onDialogueEnd -= EndDialogue;
    }

    public void AllEnemiesKilled()
    {
        if (allKilled)
            return; 

        foreach (GameObject enemy in enemies)
        {
            if(enemy != null)
            {
                return; 
            }
        }
        allKilled = true;
        Invoke("FadeIn", 2);
    }
    private void FadeIn()
    {
        TransitionCanvasManager.instance.FadeIn();
        TransitionCanvasManager.instance.onFadeIn += GoToTitleScene;
    }
    private void GoToTitleScene()
    {
        PlayerManager.Instance.player.engineSource.Stop();
        AudioManager.instance.StopLoopSound(audioSource); 
        TransitionCanvasManager.instance.onFadeIn -= GoToTitleScene;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        SceneManager.LoadScene("TitleScreen");
    }
}
