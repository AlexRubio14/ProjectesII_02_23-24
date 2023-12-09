using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;


public class UpgradeSelector : MonoBehaviour
{
    [Header("Input"), SerializeField]
    private InputActionReference upUpgradeAction;
    [SerializeField]
    private InputActionReference downUpgradeAction;
    [SerializeField]
    private InputActionReference leftUpgradeAction;
    [SerializeField]
    private InputActionReference rightUpgradeAction;

    public enum Position { UP, DOWN, RIGHT, LEFT }

    [Header("Backgrounds"), SerializeField]
    private Sprite unselectedBackground;
    [SerializeField]
    private Sprite selectedBackground;

    [SerializeField]
    private Image upBackGround; 
    [SerializeField]
    private Image downBackGround;
    [SerializeField]
    private Image rightBackGround;
    [SerializeField]
    private Image leftBackGround;

    [Space, Header("Upgrades"), SerializedDictionary("Upgrade", "Position")]
    public SerializedDictionary<Position, UpgradeObject> upgradePositions;

    [SerializeField]
    private Image upUpgradeImage;
    [SerializeField]
    private Image downUpgradeImage;
    [SerializeField]
    private Image rightUpgradeImage;
    [SerializeField]
    private Image leftUpgradeImage;


    [Space, Header("Light"), SerializeField]
    private GameObject lightUpgrade;


    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();    
    }

    private void Start()
    {
        foreach (KeyValuePair<Position, UpgradeObject> item in upgradePositions)
        {
            switch (item.Key)
            {
                case Position.UP:
                    upUpgradeImage.sprite = item.Value.c_UpgradeSprite;
                    break;
                case Position.DOWN:
                    downUpgradeImage.sprite = item.Value.c_UpgradeSprite;
                    break;
                case Position.RIGHT:
                    rightUpgradeImage.sprite = item.Value.c_UpgradeSprite;
                    break;
                case Position.LEFT:
                    leftUpgradeImage.sprite = item.Value.c_UpgradeSprite;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnEnable()
    {
        upUpgradeAction.action.started += _ => ToggleUpgrade(Position.UP);
        downUpgradeAction.action.started += _ => ToggleUpgrade(Position.DOWN);
        rightUpgradeAction.action.started += _ => ToggleUpgrade(Position.RIGHT);
        leftUpgradeAction.action.started += _ => ToggleUpgrade(Position.LEFT);
    }


    private void OnDisable()
    {
        upUpgradeAction.action.started -= _ => ToggleUpgrade(Position.UP);
        downUpgradeAction.action.started -= _ => ToggleUpgrade(Position.DOWN);
        rightUpgradeAction.action.started -= _ => ToggleUpgrade(Position.RIGHT);
        leftUpgradeAction.action.started -= _ => ToggleUpgrade(Position.LEFT);
    }


    private void ToggleUpgrade(Position _pos)
    {
        switch (upgradePositions[_pos].type)
        {
            case UpgradeObject.UpgradeType.BOOST:
                ToggleBoost(_pos);
                break;
            case UpgradeObject.UpgradeType.LIGHT:
                ToggleLight(_pos);
                break;
            case UpgradeObject.UpgradeType.DRILL:
                ToggleDrill(_pos);
                break;
            case UpgradeObject.UpgradeType.CORE_COLLECTOR:
                ToggleCoreCollector(_pos);
                break;
            default:
                break;
        }

    }
    private void ToggleBoost(Position _pos)
    {
        playerController.ChangeState(PlayerController.State.BOOST);
    }

    private void ToggleLight(Position _pos)
    {
        lightUpgrade.SetActive(!lightUpgrade.activeInHierarchy);
        ChangeBackground(_pos, lightUpgrade.activeInHierarchy);

    }

    private void ToggleDrill(Position _pos)
    {
        switch (playerController.GetState())
        {
            case PlayerController.State.IDLE:
            case PlayerController.State.MOVING:
            case PlayerController.State.INVENCIBILITY:
                playerController.ChangeState(PlayerController.State.DRILL);
                ChangeBackground(_pos, true);
                break;
            case PlayerController.State.DRILL:
                playerController.ChangeState(PlayerController.State.MOVING);
                ChangeBackground(_pos, false);
                break;
            default:
                break;
        }
    }

    private void ToggleCoreCollector(Position _pos)
    {
        Debug.LogWarning("Core Collector no implementado");
    }

    private void ChangeBackground(Position _pos, bool _on)
    {
        Sprite currentSprite;
        if (_on)
        {
            currentSprite = selectedBackground;
        }
        else
        {
            currentSprite = unselectedBackground;
        }

        switch (_pos)
        {
            case Position.UP:
                upBackGround.sprite = currentSprite;
                break;
            case Position.DOWN:
                downBackGround.sprite = currentSprite;
                break;
            case Position.RIGHT:
                rightBackGround.sprite = currentSprite; 
                break;
            case Position.LEFT:
                leftBackGround.sprite = currentSprite;
                break;
            default:
                break;
        }
    }

}
