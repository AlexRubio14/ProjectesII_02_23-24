using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            TransitionCanvasManager.instance.FadeIn();
            TransitionCanvasManager.instance.onFadeIn += GoToGame;
        }
        else
        {
            selectTpCanvas.SetActive(true);
        }
    }

    public void SubscribeToFadeIn()
    {
        ShopMusic.instance.StopMusic();
        TransitionCanvasManager.instance.onFadeIn += menuNavegation.GoToGame;
    }

    public void GoToGame()
    {
        TransitionCanvasManager.instance.onFadeIn -= GoToGame;
        SceneManager.LoadScene("Map");
    }
}
