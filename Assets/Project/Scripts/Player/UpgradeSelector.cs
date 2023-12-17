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

    [Space, Header("Upgrades"), SerializedDictionary("Position", "Upgrade")]
    public SerializedDictionary<Position, UpgradeObject> upgradePositions;
    private Dictionary<UpgradeObject, bool> obtainedUpgrades;
    [Space(height: 10), SerializedDictionary("Position", "Image")]
    public SerializedDictionary<Position, Image> upgradeImagePositions;
    [SerializeField]
    private Sprite locketUpgradeSprite;
    public bool[] upgradesToggled { private set; get; }

    [Space, Header("Boost"), SerializeField]
    private float boostMovementSpeed;
    [Space, Header("Drill"), SerializeField]
    private float drillMovementSlow;
    [SerializeField]
    private GameObject drillSprite;
    private DrillController drillController;

    [Space, Header("Light"), SerializeField]
    private GameObject lightUpgrade;

    [Space, Header("Fuel Intake"), SerializeField]
    private float boostIntake;
    [SerializeField]
    private float drillIntake;
    [SerializeField]
    private float lightIntake;


    private PlayerController playerController;

    private WebController webController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();  
        drillController = GetComponent<DrillController>();
        webController = GetComponent<WebController>();
    }

    private void Start()
    {
        obtainedUpgrades = new Dictionary<UpgradeObject, bool>();
        foreach (KeyValuePair<Position, UpgradeObject> item in upgradePositions)
        {
            Sprite currentSprite;
            if (UpgradeManager.Instance.CheckObtainedUpgrade(item.Value))
            {
                currentSprite = item.Value.c_UpgradeSprite;
                obtainedUpgrades[item.Value] = true;
            }
            else
            {
                currentSprite = locketUpgradeSprite;
                obtainedUpgrades[item.Value] = false;
            }

            upgradeImagePositions[item.Key].sprite = currentSprite;
        }


        //SetBackgroundFill(Position.DOWN, 0.3f);

        upgradesToggled = new bool[4];

    }

    private void OnEnable()
    {
        upUpgradeAction.action.started += _ => ToggleUpgrade(Position.UP);
        upUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.UP);

        downUpgradeAction.action.started += _ => ToggleUpgrade(Position.DOWN);
        downUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.DOWN);

        rightUpgradeAction.action.started += _ => ToggleUpgrade(Position.RIGHT);
        rightUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.RIGHT);

        leftUpgradeAction.action.started += _ => ToggleUpgrade(Position.LEFT);
        leftUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.LEFT);

    }
    private void OnDisable()
    {
        upUpgradeAction.action.started -= _ => ToggleUpgrade(Position.UP);
        upUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.UP);

        downUpgradeAction.action.started -= _ => ToggleUpgrade(Position.DOWN);
        downUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.DOWN);

        rightUpgradeAction.action.started -= _ => ToggleUpgrade(Position.RIGHT);
        rightUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.RIGHT);

        leftUpgradeAction.action.started -= _ => ToggleUpgrade(Position.LEFT);
        leftUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.LEFT);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (KeyValuePair<Position, UpgradeObject> item in upgradePositions)
            {
                upgradeImagePositions[item.Key].sprite = item.Value.c_UpgradeSprite;
                obtainedUpgrades[item.Value] = true;
            }
        }
    }

    private void ToggleUpgrade(Position _pos)
    {
        if (!obtainedUpgrades[upgradePositions[_pos]])
            return;
        
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
        //Upgrade 0

        if (!upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST])
        {
            webController.EraseAllWebs();
            playerController.SubstractHealth(boostIntake);
            ChangeBackground(_pos, true);
            playerController.externalMovementSpeed += boostMovementSpeed;
            upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] = true;
            //Sumar al consumo de fuel

        }
        else
        {
            ChangeBackground(_pos, false); 
            playerController.externalMovementSpeed -= boostMovementSpeed;
            upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] = false;
            //Restar al consumo de fuel

        }
    }

    private void ToggleLight(Position _pos)
    {
        if (!upgradesToggled[(int)UpgradeObject.UpgradeType.LIGHT])
        {
            lightUpgrade.SetActive(true);
            upgradesToggled[(int)UpgradeObject.UpgradeType.LIGHT] = true;
            ChangeBackground(_pos, lightUpgrade.activeInHierarchy);
            //Sumar al consumo de fuel

        }
        else
        {
            lightUpgrade.SetActive(false);
            upgradesToggled[(int)UpgradeObject.UpgradeType.LIGHT] = false;
            ChangeBackground(_pos, lightUpgrade.activeInHierarchy);
            //Restar al consumo de fuel
        }
    }

    private void ToggleDrill(Position _pos)
    {
        if (!upgradesToggled[(int)UpgradeObject.UpgradeType.DRILL])
        {
            playerController.externalMovementSpeed -= drillMovementSlow;
            drillController.enabled = true;
            drillSprite.SetActive(true);
            upgradesToggled[(int)UpgradeObject.UpgradeType.DRILL] = true;
            ChangeBackground(_pos, true);
            //Sumar al consumo de fuel

        }
        else
        {
            playerController.externalMovementSpeed += drillMovementSlow;
            drillController.enabled = false;
            drillSprite.SetActive(false);
            upgradesToggled[(int)UpgradeObject.UpgradeType.DRILL] = false;
            ChangeBackground(_pos, false);
            //Restar al consumo de fuel

        }
    }

    private void ToggleCoreCollector(Position _pos)
    {
        //Upgrade 3
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

    private void SetBackgroundFill(Position _pos, float _value)
    {
        switch (_pos)
        {
            case Position.UP:
                upBackGround.fillAmount = _value;
                break;
            case Position.DOWN:
                downBackGround.fillAmount = _value;
                break;
            case Position.RIGHT:
                rightBackGround.fillAmount = _value;
                break;
            case Position.LEFT:
                leftBackGround.fillAmount = _value;
                break;
            default:
                break;
        }
    }
}
