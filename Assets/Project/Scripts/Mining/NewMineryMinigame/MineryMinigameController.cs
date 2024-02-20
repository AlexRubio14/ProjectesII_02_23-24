using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MineryMinigameController : MonoBehaviour
{
    [SerializeField]
    private Canvas[] otherCanvas;
    [SerializeField]
    private GameObject virtualMoseObj;

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

    [HideInInspector]
    public MineralController currentMineral;
    [SerializeField]
    private SelectedMineralController selectedMineralControler;

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

        //Settearlo todo
        currentRockHealth = rockBaseHealth;

        foreach (Canvas item in otherCanvas)
        {
            item.gameObject.SetActive(false);
        }

        rightLaserOn = false;
        rightLaserRayProgress = 0;
        leftLaserOn = false;
        leftLaserRayProgress = 0;

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
    public void StartMining(MineralController _mineral)
    {
        TimeManager.Instance.PauseGame();

        currentMineral = _mineral;
        selectedMineralControler.enabled = true;
        mineralsHealth = new Dictionary<Transform, float>();

        gameObject.SetActive(true);
        virtualMoseObj.SetActive(true);




        //Inicializamos el systema de mineria asignando todos los valores acorde al mineral que estamos minando
        for (int i = 0; i < selectedMineralControler.minerals.Length; i++)
        {
            Image itemImage = selectedMineralControler.minerals[i].GetComponent<Image>();

            if (i < currentMineral.MaxItemsToReturn)
            {
                itemImage.enabled = true;
                itemImage.sprite = currentMineral.c_currentItem.c_PickableSprite;
                itemImage.color = Color.white;
                mineralsHealth.Add(selectedMineralControler.minerals[i], currentMineral.mineralsHealth[i]);
                selectedMineralControler.activeMinerals[selectedMineralControler.minerals[i]] = true;
            }
            else
            {
                itemImage.enabled = false;
                selectedMineralControler.activeMinerals[selectedMineralControler.minerals[i]] = false;
            }
        }
        
    }
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
            //Minar mineral en caso de que se usen 2 rayos bajara la vida del mineral un 50% mas rapido
            mineralsHealth[selectedMineralControler.selectedMineral] -= (mineralBreakSpeed + (mineralBreakSpeed * 0.5f * (_usedLasers - 1))) * Time.deltaTime;


            //Si se usan los 2 laseres romper un poco la roca
            if (_usedLasers == 2)
                currentRockHealth -= (rockBreakSpeed * 0.5f) * Time.deltaTime;
        }
        else
        {
            //Romper aun mas la roca
            currentRockHealth -= (rockBreakSpeed * 2 * _usedLasers) * Time.deltaTime;
        }

        CheckIfMineralMined();

    }
    private void CheckIfMineralMined()
    {
        if (mineralsHealth.ContainsKey(selectedMineralControler.selectedMineral) && mineralsHealth[selectedMineralControler.selectedMineral] <= 0)
        {
            MineMineral();
        }
    }

    private void MineMineral()
    {
        if (!selectedMineralControler.activeMinerals[selectedMineralControler.selectedMineral])
            return;    
        

        InventoryManager.Instance.ChangeRunItemAmount(currentMineral.c_currentItem, 1);
        selectedMineralControler.selectedMineral.GetComponent<Image>().color = Color.white.WithAlpha(0.2f);
        selectedMineralControler.activeMinerals[selectedMineralControler.selectedMineral] = false;
        if (selectedMineralControler.activeMinerals.ContainsValue(true))
            selectedMineralControler.MineralMined();
        else
            StopMining();//Parar de minar
        
    }

    private void BreakRock()
    {
        currentRockHealth -= rockBreakSpeed * Time.deltaTime;
    }
    
    private void StopMining()
    {
        selectedMineralControler.enabled = false;
        PlayerManager.Instance.player.ChangeState(PlayerController.State.IDLE);
        bool allMineralsMined = true;
        for (int i = 0; i < currentMineral.MaxItemsToReturn; i++)
        {
            currentMineral.mineralsHealth[i] = mineralsHealth[selectedMineralControler.minerals[i]];
            if (mineralsHealth[selectedMineralControler.minerals[i]] > 0)
            {
                allMineralsMined = false;
            }
        }

        if (allMineralsMined)
        {
            currentMineral.gameObject.SetActive(false);
            CameraController.Instance.AddMediumTrauma();
            PlayerManager.Instance.player.ChangeState(PlayerController.State.MOVING);

            MenuControlsHint.Instance.UpdateHintControls(null);
        }
        virtualMoseObj.SetActive(false);
        gameObject.SetActive(false);

        TimeManager.Instance.ResumeGame();

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
        if (mineralsHealth.ContainsKey(selectedMineralControler.selectedMineral))
            selectedMineralControler.selectionIcon.fillAmount = mineralsHealth[selectedMineralControler.selectedMineral] / currentMineral.c_currentItem.BaseMineralHealth;        
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
