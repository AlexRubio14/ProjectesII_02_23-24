using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MineryMinigameController : MonoBehaviour
{
    [SerializeField]
    private Canvas[] otherCanvas;

    [Space, Header("Input"), SerializeField]
    private InputActionReference leftLaserTrigger;
    [SerializeField]
    private InputActionReference rightLaserTrigger;
    [Space, SerializeField]
    private Transform pointer;

    [Space, Header("Lasers"), SerializeField]
    private float laserRayGrowSpeed;
    public bool leftLaserOn {  get; private set; }
    public bool rightLaserOn {  get; private set; }
    public float leftLaserRayProgress {  get; private set; }
    public float rightLaserRayProgress { get; private set; }

    [Space, Header("UI"), SerializeField]
    private Slider rockBreakBar;
    [SerializeField]
    private Image mineralProgressCircle;

    [Space, Header("Minigame"), SerializeField]
    private float rockBreakSpeed;
    [SerializeField]
    private float rockBaseHealth;
    private float currentRockHealth;

    [Space, SerializeField]
    private float mineralBreakSpeed;

    private Dictionary<Transform, float> mineralsHealth = new Dictionary<Transform, float>();

    //[HideInInspector]
    public ItemObject currentItem;

    private SelectedMineralController selectedMineralControler;

    private void Awake()
    {
        selectedMineralControler = GetComponentInChildren<SelectedMineralController>();
    }

    private void Start()
    {
        rockBreakBar.maxValue = rockBaseHealth;
        rockBreakBar.value = rockBaseHealth;
        currentRockHealth = rockBaseHealth;
    }

    private void OnEnable()
    {
        //Inputs
        leftLaserTrigger.action.started += LeftLaserAction;
        leftLaserTrigger.action.canceled += LeftLaserAction; 
        rightLaserTrigger.action.started += RightLaserAction;
        rightLaserTrigger.action.canceled += RightLaserAction;

        //Cargar los datos desde el ItemObject
        foreach (Transform item in selectedMineralControler.minerals)
        {
            item.GetComponent<Image>().sprite = currentItem.c_PickableSprite;
            mineralsHealth.Add(item, currentItem.BaseMineralHealth);
        }

        //Settearlo todo
        foreach (Canvas item in otherCanvas)
        {
            item.gameObject.SetActive(false);
        }

    }
    private void OnDisable()
    {
        //Inputs
        leftLaserTrigger.action.started -= LeftLaserAction;
        leftLaserTrigger.action.canceled -= LeftLaserAction;
        rightLaserTrigger.action.started -= RightLaserAction;
        rightLaserTrigger.action.canceled -= RightLaserAction;

        //Prepararlo todo para la proxima vez y reactivar lo necesario para el gameplay
        mineralsHealth.Clear();
        foreach (Canvas item in otherCanvas)
        {
            item.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    private void Update()
    {
        int usedLasers = CheckLasersHitted();
        CheckWhereLasersHit(usedLasers);
        SetLaserRaysProcess();
        BreakRock();
        UpdateUIInfo(usedLasers);
    }

    #region Minigame Gameplay
    private int CheckLasersHitted()
    {
        int usedLasers = 0;
        usedLasers = rightLaserOn ? usedLasers + 1 : usedLasers;
        usedLasers = leftLaserOn ? usedLasers + 1 : usedLasers;

        return usedLasers;
    }
    private void CheckWhereLasersHit(int _usedLasers)
    {
        if (_usedLasers == 0)
            return;

        //Si la distancia entre el puntero y el mineral brillante es menor a la distancia de seleccion

        if (Vector2.Distance(pointer.transform.position, selectedMineralControler.selectedMineral.position) <= selectedMineralControler.SelectionRadius
            && selectedMineralControler.hittableMineral)
        {
            //Minar mineral

            //Si se usan los 2 laseres romper un poco la roca
            if (_usedLasers == 2)
                currentRockHealth -= (rockBreakSpeed * 0.5f) * Time.deltaTime;

        }
        else
        {
            //Romper aun mas la roca
            currentRockHealth -= (rockBreakSpeed * 2 * _usedLasers) * Time.deltaTime;
        }

    }
    private void BreakRock()
    {
        currentRockHealth -= rockBreakSpeed * Time.deltaTime;
    }
    #endregion

    #region Laser Rays
    private void SetLaserRaysProcess()
    {
        leftLaserRayProgress += leftLaserOn ? laserRayGrowSpeed * Time.deltaTime : -laserRayGrowSpeed * Time.deltaTime;
        rightLaserRayProgress += rightLaserOn ? laserRayGrowSpeed * Time.deltaTime : -laserRayGrowSpeed * Time.deltaTime;

        leftLaserRayProgress = Mathf.Clamp01(leftLaserRayProgress);
        rightLaserRayProgress = Mathf.Clamp01(rightLaserRayProgress);
    }
    #endregion

    #region UI
    private void UpdateUIInfo(int _userLasers)
    {
        rockBreakBar.value = currentRockHealth;

        
    }
    #endregion


    #region Input
    private void LeftLaserAction(InputAction.CallbackContext obj)
    {
        leftLaserOn = obj.action.IsPressed();
    }
    private void RightLaserAction(InputAction.CallbackContext obj)
    {
        rightLaserOn = obj.action.IsInProgress();
    }
    #endregion
}
