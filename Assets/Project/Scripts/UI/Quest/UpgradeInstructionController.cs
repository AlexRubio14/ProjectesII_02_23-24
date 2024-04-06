using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeInstructionController : MonoBehaviour
{
    [SerializedDictionary("Upgrade", "Instruction")]
    public SerializedDictionary<UpgradeObject, Sprite[]> upgradeInstructions;
    [SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;


    [SerializeField]
    private TextMeshProUGUI questNameText;
    [SerializeField]
    private TextMeshProUGUI questDescriptionText;

    [SerializeField]
    private Image questInstructionImage;
    [SerializeField]
    private Image questInputImage;

    private void OnEnable()
    {
        InputSystem.onDeviceChange += UpdateInputImages;

        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateInputImages;
    }


    public void SetUpgradeInstructions(UpgradeObject currentUpgrade)
    {
        questNameText.text = currentUpgrade.UpgradeName;
        questDescriptionText.text = currentUpgrade.UpgradeDescription;

        questInstructionImage.sprite = upgradeInstructions[currentUpgrade][0];
        questInputImage.sprite = upgradeInstructions[currentUpgrade][(int)InputController.Instance.GetControllerType() + 1];
    }

    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }

}
