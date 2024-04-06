using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuNavegation : MonoBehaviour
{
    [SerializeField]
    private string hubScene;
    [SerializeField]
    private string titleScene;
    [SerializeField]
    private string gameScene;


    private void Start()
    {
        PlayerPrefs.Save();
    }

    public void GoToHub()
    {
        SceneManager.LoadScene(hubScene);
    }

    public void GoToTitleScreen()
    {
        SceneManager.LoadScene(titleScene);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
