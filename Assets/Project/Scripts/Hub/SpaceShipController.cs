using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{
    [SerializeField]
    private GameObject selectTpCanvas;
    [SerializeField]
    private MenuNavegation menuNavegation;

    public void PressSpaceShip()
    {
        if (!SelectTpsManager.instance.tpList[0].discovered)
        {
            SelectTpsManager.instance.SetIdToTeleport(1);
            ShopMusic.instance.StopMusic();
            menuNavegation.GoToGame();
        }
        else
        {
            selectTpCanvas.SetActive(true);
        }
    }
}
