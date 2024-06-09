using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoostTutorial : Tutorial
{
    [SerializeField]
    private Canvas boostCanvasTutorial;

    [SerializeField]
    private UpgradeObject boost;


    [Space, SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;

    private void OnEnable()
    {
        InputSystem.onDeviceChange += UpdateInputImages;
        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateInputImages;
    }

    protected override void TutorialMethod()
    {
        CheckIfHasBoost();
    }

    private void CheckIfHasBoost()
    {
        if (!UpgradeManager.Instance.CheckObtainedUpgrade(boost))
            return;

        StartBoostTutorial();
    }

    private void StartBoostTutorial()
    {
        boostCanvasTutorial.gameObject.SetActive(true);

        dialogueController.onDialogueEnd += EndTutorial;

        dialogueController.dialogues = dialogues;
        dialogueController.gameObject.SetActive(true);

        dialogueController.StartDialogue();
        TimeManager.Instance.PauseGame();
    }

    protected override void EndTutorial()
    {
        boostCanvasTutorial.enabled = false;
        TimeManager.Instance.ResumeGame();

        dialogueController.onDialogueEnd -= EndTutorial;

        PlayerPrefs.SetInt(tutorialkey, 1);

        boostCanvasTutorial.gameObject.SetActive(true);

        Destroy(this);
    }


    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }

}
