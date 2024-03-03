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
        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.ACCEPT);
        neededControls.Add(MenuControlsHint.ActionType.GO_BACK);
        neededControls.Add(MenuControlsHint.ActionType.MOVE_MENU);

        MenuControlsHint.Instance.UpdateHintControls(neededControls);

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
