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

    [SerializedDictionary("Position", "Background")]
    public SerializedDictionary<Position, Image> backgroundImagePositions;


    [Space, Header("Upgrades"), SerializedDictionary("Position", "Upgrade")]
    public SerializedDictionary<Position, UpgradeObject> upgradePositions;
    private Dictionary<UpgradeObject, bool> obtainedUpgrades;
    [Space(height: 10), SerializedDictionary("Position", "Image")]
    public SerializedDictionary<Position, Image> upgradeImagePositions;
    [Space, SerializeField]
    private Sprite locketUpgradeSprite;
    public bool[] upgradesToggled { private set; get; }

    [Space, Header("Boost"), SerializeField]
    private float boostMovementSpeed;
    private float boostStamina = 1;
    [SerializeField]
    private float boostStaminaReduction;
    [SerializeField]
    private float boostStaminaRecover;
    private Position boostPos;
    [SerializeField]
    private ParticleSystem boostParticles;

    [Space, Header("Drill"), SerializeField]
    private float drillMovementSlow;
    [SerializeField]
    private GameObject drillSprite;
    private DrillController drillController;

    [Space, Header("Light"), SerializeField]
    private GameObject lightUpgrade;

    [Space, Header("Fuel Consume"), SerializeField]
    private float boostConsume;
    [SerializeField]
    private float drillConsume;
    [SerializeField]
    private float lightConsume;


    private PlayerController playerController;
    private PlayerWebController webController;
    private AutoHelpController autoHelpController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();  
        drillController = GetComponent<DrillController>();
        webController = GetComponent<PlayerWebController>();
        autoHelpController = GetComponent<AutoHelpController>();
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

            if (item.Value.type == UpgradeObject.UpgradeType.BOOST)
            {
                boostPos = item.Key;
            }

        }

        upgradesToggled = new bool[4];

        boostParticles.Stop(true);

    }

    private void OnEnable()
    {
        upUpgradeAction.action.started += _ => ToggleUpgrade(Position.UP, _);
        upUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.UP, _);

        downUpgradeAction.action.started += _ => ToggleUpgrade(Position.DOWN, _);
        downUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.DOWN, _);

        rightUpgradeAction.action.started += _ => ToggleUpgrade(Position.RIGHT, _);
        rightUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.RIGHT, _);

        leftUpgradeAction.action.started += _ => ToggleUpgrade(Position.LEFT, _);
        leftUpgradeAction.action.canceled += _ => ToggleUpgrade(Position.LEFT, _);

    }

    private void OnDisable()
    {
        upUpgradeAction.action.started -= _ => ToggleUpgrade(Position.UP, _);
        upUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.UP, _);

        downUpgradeAction.action.started -= _ => ToggleUpgrade(Position.DOWN, _);
        downUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.DOWN, _);

        rightUpgradeAction.action.started -= _ => ToggleUpgrade(Position.RIGHT, _);
        rightUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.RIGHT, _);

        leftUpgradeAction.action.started -= _ => ToggleUpgrade(Position.LEFT, _);
        leftUpgradeAction.action.canceled -= _ => ToggleUpgrade(Position.LEFT, _);
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

        CheckBoostStamina();
    }

    private void ToggleUpgrade(Position _pos, InputAction.CallbackContext obj)
    {
        if (!obtainedUpgrades[upgradePositions[_pos]])
            return;
        
        switch (upgradePositions[_pos].type)
        {
            case UpgradeObject.UpgradeType.BOOST:
                bool isPressed = obj.action.IsPressed();
                ToggleBoost(_pos, isPressed);
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
    private void ToggleBoost(Position _pos, bool _pressed)
    {
        //Upgrade 0

        if (!upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] && _pressed)
        {
            webController.EraseAllWebs();
            ChangeBackground(_pos, true);
            playerController.externalMovementSpeed += boostMovementSpeed;
            upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] = true;
            //Sumar al consumo de fuel
            playerController.fuelConsume += boostConsume;
            boostParticles.Play(true);
        }
        else if(upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST])
        {
            ChangeBackground(_pos, false); 
            playerController.externalMovementSpeed -= boostMovementSpeed;
            upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] = false;
            //Restar al consumo de fuel
            playerController.fuelConsume -= boostConsume;
            boostParticles.Stop(true);
        }
    }
    private void CheckBoostStamina()
    {
        if (!obtainedUpgrades[upgradePositions[boostPos]])
            return;

        //Si la mejora esta activada resta stamina        
        if (upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST])
        {
            boostStamina -= boostStaminaReduction * Time.deltaTime;
        }
        else //Si esta desactivada recupera stamina
        {
            boostStamina += boostStaminaRecover * Time.deltaTime;
        }

        boostStamina = Mathf.Clamp01(boostStamina);

        backgroundImagePositions[boostPos].fillAmount = boostStamina;

        if (boostStamina == 0)
        {
            ToggleBoost(boostPos, false);
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
            playerController.fuelConsume += lightConsume;
        }
        else
        {
            lightUpgrade.SetActive(false);
            upgradesToggled[(int)UpgradeObject.UpgradeType.LIGHT] = false;
            ChangeBackground(_pos, lightUpgrade.activeInHierarchy);
            //Restar al consumo de fuel
            playerController.fuelConsume -= lightConsume;
        }
    }

    private void ToggleDrill(Position _pos)
    {
        if (!upgradesToggled[(int)UpgradeObject.UpgradeType.DRILL])
        {
            //ACTIVAMOS LA MEJORA
            playerController.externalMovementSpeed -= drillMovementSlow;
            drillController.enabled = true;
            drillSprite.SetActive(true);
            upgradesToggled[(int)UpgradeObject.UpgradeType.DRILL] = true;
            ChangeBackground(_pos, true);
            //Sumar al consumo de fuel
            playerController.fuelConsume += drillConsume;
            autoHelpController.enabled = false;
            
        }
        else
        {
            //DESACTIVAMOS LA MEJORA
            playerController.externalMovementSpeed += drillMovementSlow;
            drillController.enabled = false;
            drillSprite.SetActive(false);
            upgradesToggled[(int)UpgradeObject.UpgradeType.DRILL] = false;
            ChangeBackground(_pos, false);
            //Restar al consumo de fuel
            playerController.fuelConsume -= drillConsume;
            autoHelpController.enabled = true;
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

        backgroundImagePositions[_pos].sprite = currentSprite;
    }

}
