using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelector : MonoBehaviour
{
    public enum Position { UP, DOWN, RIGHT, LEFT }
    public enum Type { BOOST, LIGHT, DRILL, CORE_COLLECTOR }

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

    [Space, Header("Upgrades"), AYellowpaper.SerializedCollections.SerializedDictionary("Upgrade", "Position")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<Position, UpgradeObject> upgradePositions;
    [AYellowpaper.SerializedCollections.SerializedDictionary("Upgrade", "Type")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<UpgradeObject, Type> upgradeTypes;

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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleUpgrade(Position.UP);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleUpgrade(Position.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleUpgrade(Position.RIGHT);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToggleUpgrade(Position.DOWN);
        }
    }


    private void ToggleUpgrade(Position _pos)
    {
        UpgradeObject currUpgrade = upgradePositions[_pos];
        Type upgradeType = upgradeTypes[currUpgrade];

        switch (upgradeType)
        {
            case Type.BOOST:
                ToggleBoost(_pos);
                break;
            case Type.LIGHT:
                ToggleLight(_pos);
                break;
            case Type.DRILL:
                ToggleDrill(_pos);
                break;
            case Type.CORE_COLLECTOR:
                ToggleCoreCollector(_pos);
                break;
            default:
                break;
        }

    }
    private void ToggleBoost(Position _pos)
    {
        Debug.LogWarning("Boost no implementado");
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
