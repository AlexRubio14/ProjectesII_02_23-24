using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueObject;
    private TextMeshProUGUI c_dialogueText;

    [TextArea, SerializeField]
    private string[] dialogues;

    [SerializeField]
    private float timeBetweenLetters;

    private int currentDialogueIndex = 0;
    private int letterIndex;

    private bool showingText = false;
    private bool displayingDialogue = false;

    private void Awake()
    {
        c_dialogueText = dialogueObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        dialogueObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartDialogue();
        }

        if (Input.GetKeyDown(KeyCode.Space))
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
    }


    public void StartDialogue()
    {
        //Empezar con el dialogo
        dialogueObject.SetActive(true);
        c_dialogueText.text = "";
        currentDialogueIndex = 0;
        letterIndex = 0;
        showingText = true;
        displayingDialogue = true;
        Invoke("DisplayLetters", timeBetweenLetters);
    }

    private void DisplayNextDialogue()
    {
        if (dialogues.Length > currentDialogueIndex)
        {
            //Si aun no se ha acabado el dialogo
            displayingDialogue = true;
            letterIndex = 0;
            c_dialogueText.text = "";
            Invoke("DisplayLetters", timeBetweenLetters);
        }
        else
        {
            //Si no hay mas dialogos
            showingText = false;
            displayingDialogue = false;
            dialogueObject.SetActive(false);
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
                return;
            }
            c_dialogueText.text += dialogues[currentDialogueIndex][letterIndex];
            letterIndex++;
            Invoke("DisplayLetters", timeBetweenLetters);
        }
    }

    private void DisplayAllLetters()
    {
        displayingDialogue = false;
        c_dialogueText.text = dialogues[currentDialogueIndex];
        currentDialogueIndex++;

    }

}
