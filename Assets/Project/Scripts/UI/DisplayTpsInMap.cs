using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayTpsInMap : DisplayTps
{
    private PlayerTpController playerTpController;

    [Header("Inputs"), SerializeField]
    private InputActionReference resumeAction;

    private Vector2 positionToTravel;

    SelectTpController[] selectTpControllers;


    protected void OnEnable()
    {
        resumeAction.action.started += ResumeGame;
    }
    private void OnDisable()
    {
        resumeAction.action.started -= ResumeGame;
    }

    private void Start()
    {
        CreateDiscoveredTpList();
        playerTpController = PlayerManager.Instance.player.GetComponent<PlayerTpController>();
        selectTpControllers = FindObjectsOfType<SelectTpController>();
    }

    public override void OnButtonClick(int id)
    {
        tpMenu.SetActive(false);

        foreach (SelectTpController item in selectTpControllers)
        {
            if(item.id == id)
            {
                positionToTravel = item.tpPosition.position;
                break;
            }
        }

        playerTpController.onTpStop += TravelToTp;
        playerTpController.StartTravel();

    }

    private void TravelToTp()
    {
        playerTpController.transform.position = positionToTravel;
        playerTpController.EnablePlayer(true);
        playerTpController.DisplayCanvas(true);

        PlayerManager.Instance.player.ChangeState(PlayerController.State.IDLE);
        ResumeGameButton();

        playerTpController.onTpStop -= TravelToTp;
    }

    public void OpenMenu()
    {
        InputController.Instance.ChangeActionMap("Menu");
        TimeManager.Instance.PauseGame();

        List<MenuControlsHint.ActionType> actions = new List<MenuControlsHint.ActionType>();
        actions.Add(MenuControlsHint.ActionType.MOVE_MENU);
        actions.Add(MenuControlsHint.ActionType.ACCEPT);
        actions.Add(MenuControlsHint.ActionType.GO_BACK);
        MenuControlsHint.Instance.UpdateHintControls(actions);
    }

    public void ReturnToHub()
    {
        ResumeGameButton();
        playerTpController.onTpStop += EndTpReturnHub;
        playerTpController.StartTravel();
    }

    private void EndTpReturnHub()
    {
        InventoryManager.Instance.EndRun(true);
        ResumeGameButton();
        menuNavegation.GoToHub();
        playerTpController.onTpStop -= EndTpReturnHub;
    }

    public void ResumeGameButton()
    {
        ResumeGame(new InputAction.CallbackContext());
    }

    public void ResumeGame(InputAction.CallbackContext obj)
    {
        InputController.Instance.ChangeActionMap("Player");
        TimeManager.Instance.ResumeGame();
        tpMenu.gameObject.SetActive(false);

        MenuControlsHint.Instance.UpdateHintControls(null);
    }
}
