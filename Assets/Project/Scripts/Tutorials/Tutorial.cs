using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tutorial : MonoBehaviour
{
    [SerializeField]
    protected string tutorialkey;

    [TextArea, SerializeField]
    public string[] dialogues;

    [SerializeField]
    protected DialogueController dialogueController;

    protected void Awake()
    {
        if (PlayerPrefs.HasKey(tutorialkey))
        {
            Destroy(this);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerPrefs.HasKey(tutorialkey))
                return;

            TutorialMethod();
        }
    }

    protected abstract void TutorialMethod();

    protected abstract void EndTutorial();
}
