using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeededUpgradePlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject c_itemToShowCanvas;
    [SerializeField]
    private Image c_itemToShowImage;
    [SerializeField]
    private GameObject c_lockedUpgradeImage;

    [SerializeField]
    private Sprite interactKeySprite;

    public void ShowNeededUpgrade(UpgradeObject _upgrade)
    {
        c_itemToShowCanvas.SetActive(true);
        if (_upgrade && !UpgradeManager.Instance.UpgradeObtained[_upgrade])
        {
            c_itemToShowImage.sprite = _upgrade.c_UpgradeSprite;
            c_lockedUpgradeImage.SetActive(true);
            return;
        }

        //Mostrar el sprite de pulsar el boton
        c_itemToShowImage.sprite = interactKeySprite;
        c_lockedUpgradeImage.SetActive(false);


    }

    public void HideNeededUpgrade()
    {
        c_itemToShowCanvas.SetActive(false);
    }
}
