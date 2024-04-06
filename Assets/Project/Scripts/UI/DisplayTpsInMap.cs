using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DisplayTpsInMap : DisplayTps
{
    private PlayerTpController playerTpController;

    [Header("Inputs"), SerializeField]
    private InputActionReference resumeAction;
    [SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;

    private Vector2 positionToTravel;

    SelectTpController[] selectTpControllers;


    protected void OnEnable()
    {
        resumeAction.action.started += ResumeGame;
        InputSystem.onDeviceChange += UpdateInputImages;
        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);
    }
    private void OnDisable()
    {
        resumeAction.action.started -= ResumeGame;
        InputSystem.onDeviceChange -= UpdateInputImages;
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
    }

    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }
}
