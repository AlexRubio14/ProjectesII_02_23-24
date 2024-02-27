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

    [Header("Audio Boost"), SerializeField]
    private AudioClip startBoost;
    [SerializeField]
    private AudioClip boost;
    [SerializeField]
    private AudioClip finishBoost;
    private AudioSource boostSource;

    [Header("Light"), SerializeField]
    private AudioClip SwitchLightClip;
    [SerializeField]
    private AudioClip loopLightClip;
    private AudioSource loopLightSource;

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

    //[Space, Header("Size Changer"), SerializeField]
    private SizeUpgradeController sizeUpgrade;

    [Space, Header("Fuel Consume"), SerializeField]
    private float boostConsume;
    [SerializeField]
    private float drillConsume;
    [SerializeField]
    private float lightConsume;
    [SerializeField]
    private float sizeChangerConsume;


    private PlayerController playerController;
    private AutoHelpController autoHelpController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();  
        drillController = GetComponent<DrillController>();
        autoHelpController = GetComponent<AutoHelpController>();
        sizeUpgrade = GetComponent<SizeUpgradeController>();
    }

    private void Start()
    {
        obtainedUpgrades = new Dictionary<UpgradeObject, bool>();


        foreach (KeyValuePair<Position, UpgradeObject> item in upgradePositions)
        {
            Sprite currentSprite;
            if (UpgradeManager.Instance.CheckObtainedUpgrade(item.Value))
            {
                currentSprite = item.Value.UpgradeSprite;
                obtainedUpgrades[item.Value] = true;
            }
            else
            {
                currentSprite = locketUpgradeSprite;
                obtainedUpgrades[item.Value] = false;
            }

            upgradeImagePositions[item.Key].sprite = currentSprite;

            if (item.Value.type == UpgradeObject.UpgradeType.BOOST)
                boostPos = item.Key;
            
        }

        upgradesToggled = new bool[4];

        boostParticles.Stop(true);
    }

    private void OnEnable()
    {
        upUpgradeAction.action.started += upUpgradeStarted => ToggleUpgrade(Position.UP, upUpgradeStarted);
        upUpgradeAction.action.canceled += upUpgradeCanceled => ToggleUpgrade(Position.UP, upUpgradeCanceled);

        downUpgradeAction.action.started += downUpgradeStarted => ToggleUpgrade(Position.DOWN, downUpgradeStarted);
        downUpgradeAction.action.canceled += downUpgradeCanceled => ToggleUpgrade(Position.DOWN, downUpgradeCanceled);

        rightUpgradeAction.action.started += rightUpgradeStarted => ToggleUpgrade(Position.RIGHT, rightUpgradeStarted);
        rightUpgradeAction.action.canceled += rightUpgradeCanceled => ToggleUpgrade(Position.RIGHT, rightUpgradeCanceled);

        leftUpgradeAction.action.started += leftUpgradeStarted => ToggleUpgrade(Position.LEFT, leftUpgradeStarted);
        leftUpgradeAction.action.canceled += leftUpgradeCanceled => ToggleUpgrade(Position.LEFT, leftUpgradeCanceled);

    }

    private void OnDisable()
    {
        upUpgradeAction.action.started -= upUpgradeStarted => ToggleUpgrade(Position.UP, upUpgradeStarted);
        upUpgradeAction.action.canceled -= upUpgradeCanceled => ToggleUpgrade(Position.UP, upUpgradeCanceled);

        downUpgradeAction.action.started -= downUpgradeStarted => ToggleUpgrade(Position.DOWN, downUpgradeStarted);
        downUpgradeAction.action.canceled -= downUpgradeCanceled => ToggleUpgrade(Position.DOWN, downUpgradeCanceled);

        rightUpgradeAction.action.started -= rightUpgradeStarted => ToggleUpgrade(Position.RIGHT, rightUpgradeStarted);
        rightUpgradeAction.action.canceled -= rightUpgradeCanceled => ToggleUpgrade(Position.RIGHT, rightUpgradeCanceled);

        leftUpgradeAction.action.started -= leftUpgradeStarted => ToggleUpgrade(Position.LEFT, leftUpgradeStarted);
        leftUpgradeAction.action.canceled -= leftUpgradeCanceled => ToggleUpgrade(Position.LEFT, leftUpgradeCanceled);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (KeyValuePair<Position, UpgradeObject> item in upgradePositions)
            {
                upgradeImagePositions[item.Key].sprite = item.Value.UpgradeSprite;
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
            case UpgradeObject.UpgradeType.SIZE_CHANGER:
                ToggleSizeChanger(_pos);
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
            ChangeBackground(_pos, true);
            playerController.externalMovementSpeed += boostMovementSpeed;
            upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] = true;
            //Restar el consumo de fuel
            playerController.fuelConsume -= boostConsume;
            boostParticles.Play(true);
            AudioManager._instance.Play2dOneShotSound(startBoost, "Boost");
            boostSource = AudioManager._instance.Play2dLoop(boost, "Boost");

        }
        else if(upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST])
        {
            
            ChangeBackground(_pos, false); 
            playerController.externalMovementSpeed -= boostMovementSpeed;
            upgradesToggled[(int)UpgradeObject.UpgradeType.BOOST] = false;
            //Resetear el consumo de fuel sumando
            playerController.fuelConsume += boostConsume;
            boostParticles.Stop(true);
            AudioManager._instance.StopLoopSound(boostSource);
            AudioManager._instance.Play2dOneShotSound(finishBoost, "Boost");
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
            //Restar el consumo de fuel
            playerController.fuelConsume -= lightConsume;
            AudioManager._instance.Play2dOneShotSound(SwitchLightClip, "Light");
            loopLightSource = AudioManager._instance.Play2dLoop(loopLightClip, "Light");
        }
        else
        {
            StartCoroutine(AudioManager._instance.FadeOutSFXLoop(loopLightSource));
            lightUpgrade.SetActive(false);
            upgradesToggled[(int)UpgradeObject.UpgradeType.LIGHT] = false;
            ChangeBackground(_pos, lightUpgrade.activeInHierarchy);
            //Resetear el consumo de fuel sumando

            playerController.fuelConsume += lightConsume;
            AudioManager._instance.Play2dOneShotSound(SwitchLightClip, "Light");
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
            //Restar el consumo de fuel
            playerController.fuelConsume -= drillConsume;
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
            //Resetear el consumo de fuel sumando
            playerController.fuelConsume += drillConsume;
            autoHelpController.enabled = true;
        }
    }
    private void ToggleSizeChanger(Position _pos)
    {
        //Upgrade 3
        if (!upgradesToggled[(int)UpgradeObject.UpgradeType.SIZE_CHANGER])
        {
            //ACTIVAMOS LA MEJORA
            upgradesToggled[(int)UpgradeObject.UpgradeType.SIZE_CHANGER] = true;
            sizeUpgrade.enabled = true;
            sizeUpgrade.SetGrowing(false);
            ChangeBackground(_pos, true);
            //Restar el consumo de fuel
            playerController.fuelConsume -= sizeChangerConsume;
            autoHelpController.enabled = false;
        }
        else
        {
            //DESACTIVAMOS LA MEJORA
            upgradesToggled[(int)UpgradeObject.UpgradeType.SIZE_CHANGER] = false;
            sizeUpgrade.SetGrowing(true);

            ChangeBackground(_pos, false);
            //Resetear el consumo de fuel sumando
            playerController.fuelConsume += sizeChangerConsume;
        }
        
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
