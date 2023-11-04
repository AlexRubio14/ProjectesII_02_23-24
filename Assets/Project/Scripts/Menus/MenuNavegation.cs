using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuNavegation : MonoBehaviour
{
    public void GoToHub()
    {
        SceneManager.LoadScene("ShopScene"); 
    }

    public void GoToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("PlayableMapScene");
    }

    public void Exit()
    {
        Debug.Log("Exit game..."); 
        Application.Quit();
    }
}
