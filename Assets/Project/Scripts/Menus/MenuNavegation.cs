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
        TransitionCanvasManager.instance.FadeOut();
    }

    public void GoToHub()
    {
        SceneManager.LoadScene(hubScene);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void GoToTitleScreen()
    {
        SceneManager.LoadScene(titleScene);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(gameScene);
        TransitionCanvasManager.instance.onFadeIn -= GoToGame;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void Exit()
    {
        Application.Quit();
    }
}
