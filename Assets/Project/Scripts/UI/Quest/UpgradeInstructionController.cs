using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeInstructionController : MonoBehaviour
{
    [SerializedDictionary("Upgrade", "Instruction")]
    public SerializedDictionary<UpgradeObject, Sprite[]> upgradeInstructions;

    [SerializeField]
    private TextMeshProUGUI questNameText;
    [SerializeField]
    private TextMeshProUGUI questDescriptionText;

    [SerializeField]
    private Image questInstructionImage;
    [SerializeField]
    private Image questInputImage;

    public void SetUpgradeInstructions(UpgradeObject currentUpgrade)
    {
        questNameText.text = currentUpgrade.UpgradeName;
        questDescriptionText.text = currentUpgrade.UpgradeDescription;

        questInstructionImage.sprite = upgradeInstructions[currentUpgrade][0];
        questInputImage.sprite = upgradeInstructions[currentUpgrade][(int)InputController.Instance.GetCurrentControllerType() + 1];
    }
}
