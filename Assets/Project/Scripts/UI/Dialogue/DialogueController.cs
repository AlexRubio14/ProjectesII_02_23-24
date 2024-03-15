using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private PlayerInput menuInput;
    private string lastActionMap;
    [SerializeField]
    private InputActionReference dialogueAction;

    [Space, SerializeField]
    private GameObject dialogueObject;
    private TextMeshProUGUI c_dialogueText;

    [TextArea, SerializeField]
    public string[] dialogues;

    [SerializeField]
    private float timeBetweenLetters;

    private int currentDialogueIndex = 0;
    private int letterIndex;

    private bool showingText = false;
    private bool displayingDialogue = false;
    [SerializeField]
    private MenuControlsHint.HintsPos hintsPos = MenuControlsHint.HintsPos.BOTTOM_LEFT;

    [Space, Header("Animator"), SerializeField]
    private Animator[] catAnimations;
    [SerializeField]
    private AudioClip catSound;
    [SerializeField]
    private AudioClip catDialogueSound;
    private int dialogueSoundIndex;
    [SerializeField]
    private bool changeDialoguesHint = true;

    [HideInInspector]
    public Action onDialogueEnd;
    [HideInInspector]
    public Action<int> onDialogueLineStart;

    private void Awake()
    {
        c_dialogueText = dialogueObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        //Activar Input
        dialogueAction.action.started += InputPressed;

        lastActionMap = menuInput.currentActionMap.name;
        menuInput.SwitchCurrentActionMap("Dialogue");

        List<MenuControlsHint.ActionType> actionList = new List<MenuControlsHint.ActionType>();
        actionList.Add(MenuControlsHint.ActionType.SKIP_DIALOGUE);

        if (MenuControlsHint.Instance && changeDialoguesHint)
            MenuControlsHint.Instance.UpdateHintControls(actionList, null, hintsPos);

    }

    private void OnDisable()
    {
        //Desactivar Input
        dialogueAction.action.started -= InputPressed;
        menuInput.SwitchCurrentActionMap(lastActionMap);

        if(MenuControlsHint.Instance && changeDialoguesHint)
            MenuControlsHint.Instance.UpdateHintControls(MenuControlsHint.Instance.lastActions);
    }

    private void InputPressed(InputAction.CallbackContext obj)
    {
        if (showingText)
        {
            if (displayingDialogue)
            {
                DisplayAllLetters();
            }
            else
            {
                DisplayNextDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        if (dialogues.Length == 0)
        {
            showingText = false;
            displayingDialogue = false;
            if(onDialogueEnd != null)
                onDialogueEnd();
            gameObject.SetActive(false);
            return;
        }


        //Empezar con el dialogo
        dialogueObject.SetActive(true);
        currentDialogueIndex = 0;
        letterIndex = 0;
        c_dialogueText.text = dialogues[currentDialogueIndex];
        c_dialogueText.maxVisibleCharacters = letterIndex;
        showingText = true;
        displayingDialogue = true;
        Invoke("DisplayLetters", timeBetweenLetters);

        foreach (Animator animator in catAnimations)
        {
            animator.SetBool("talking", true);
            animator.SetTrigger("normalTalk"); 
        }

    }

    private void DisplayNextDialogue()
    {

        if (dialogues.Length > currentDialogueIndex)
        {
            foreach (Animator animator in catAnimations)
            {
                animator.SetBool("talking", true);
                animator.SetTrigger("normalTalk");
            }
            //Si aun no se ha acabado el dialogo
            displayingDialogue = true;
            letterIndex = 0;
            c_dialogueText.text = dialogues[currentDialogueIndex];
            if(onDialogueLineStart != null)
                onDialogueLineStart(currentDialogueIndex);

            c_dialogueText.maxVisibleCharacters = letterIndex;
            Invoke("DisplayLetters", timeBetweenLetters);
            

        }
        else
        {
            //Si no hay mas dialogos
            showingText = false;
            displayingDialogue = false;
            if(onDialogueEnd != null)
                onDialogueEnd();
            gameObject.SetActive(false);
        }

    }
    private void DisplayLetters()
    {
        if (displayingDialogue)
        {
            
            if (letterIndex >= dialogues[currentDialogueIndex].Length)
            {
                //Exit
                currentDialogueIndex++;
                displayingDialogue = false;
                foreach (Animator animator in catAnimations)
                {
                    animator.SetBool("talking", false);
                }
            }
            c_dialogueText.maxVisibleCharacters = letterIndex;
            letterIndex++;
            Invoke("DisplayLetters", timeBetweenLetters);
            if(dialogueSoundIndex % 4 == 0)
                AudioManager.instance.Play2dOneShotSound(catDialogueSound, "SFX", 0.35f, 2f, 2.5f);

            dialogueSoundIndex++;
        }
    }

    private void DisplayAllLetters()
    {
        displayingDialogue = false;
        c_dialogueText.maxVisibleCharacters = dialogues[currentDialogueIndex].Length;
        currentDialogueIndex++;
        foreach (Animator animator in catAnimations)
        {
            animator.SetBool("talking", false);
        }
    }

}
