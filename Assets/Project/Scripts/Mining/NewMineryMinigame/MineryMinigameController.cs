using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;

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

    [Space, Header("Random Mineral Position"), SerializeField]
    private Transform neutralPosition;
    [SerializeField]
    private Transform maxPosition;
    [SerializeField]
    private Transform minPosition;
    [Space, SerializeField]
    private float minDistanceBetweenMinerals;
    [SerializeField]
    private float randomRotation;

    [Space, Header("Minigame"), SerializeField]
    private float rockBreakSpeed;
    [field: SerializeField]
    public float mineralMissedRockDamage {  get; private set; }

    [Space, SerializeField]
    private float mineralBreakSpeed;

    private Dictionary<Transform, float> mineralsHealth = new Dictionary<Transform, float>();

    [HideInInspector]
    public MineralController currentMineral;
    [SerializeField]
    private SelectedMineralController selectedMineralControler;

    private void OnEnable()
    {
        //Inputs
        leftLaserTrigger.action.started += LeftLaserAction;
        leftLaserTrigger.action.canceled += LeftLaserAction; 
        rightLaserTrigger.action.started += RightLaserAction;
        rightLaserTrigger.action.canceled += RightLaserAction;        

        foreach (Canvas item in otherCanvas)
            item.gameObject.SetActive(false);

        rightLaserOn = false;
        rightLaserRayProgress = 0;
        leftLaserOn = false;
        leftLaserRayProgress = 0;

        InputSystem.onDeviceChange += UpdateInputImages;
        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);

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
            item.gameObject.SetActive(true);

        InputSystem.onDeviceChange -= UpdateInputImages;
    }

    // Update is called once per frame
    private void Update()
    {
        int usedLasers = CheckLasersHitted();
        CheckWhereLasersHit(usedLasers);
        SetLaserRaysProcess();
        BreakRock();
        UpdateUIInfo(usedLasers);

        if (Input.GetKeyDown(KeyCode.L))
        {
            StopMining(false);
        }

    }

    #region Place Minerals
    private void SetRandomPositionMinerals()
    {
        //Primero Reiniciamos las posiciones de los minerales a un punto neutro
        for (int i = 0; i < selectedMineralControler.minerals.Length; i++)
        {
            if (!mineralsHealth.ContainsKey(selectedMineralControler.minerals[i]))
                break;

            selectedMineralControler.minerals[i].position = neutralPosition.position;

        }

        for (int i = 0; i < selectedMineralControler.minerals.Length; i++)
        {
            if (!mineralsHealth.ContainsKey(selectedMineralControler.minerals[i]))
                break;

            selectedMineralControler.minerals[i].position = GetRandomPosition(selectedMineralControler.minerals[i]);
            selectedMineralControler.minerals[i].rotation = Quaternion.Euler(0,0, Random.Range(-randomRotation, randomRotation));
        }
    }

    private Vector3 GetRandomPosition(Transform _currentTransform)
    {
        float randomX = Random.Range(minPosition.position.x, maxPosition.position.x);
        float randomY = Random.Range(minPosition.position.y, maxPosition.position.y);

        Vector3 randomPos = new Vector3(randomX, randomY, _currentTransform.position.z);

        for (int i = 0; i < selectedMineralControler.minerals.Length; i++)
        {
            if (!mineralsHealth.ContainsKey(selectedMineralControler.minerals[i]))
                break;

            if (selectedMineralControler.minerals[i] != _currentTransform &&
                Vector2.Distance(selectedMineralControler.minerals[i].position, randomPos) < minDistanceBetweenMinerals)
            {
                return GetRandomPosition(_currentTransform);
            }

        }

        return randomPos;
    }

    #endregion

    #region Minigame Gameplay
    public void StartMining(MineralController _mineral)
    {
        TimeManager.Instance.PauseGame();

        currentMineral = _mineral;

        rockBreakBar.maxValue = currentMineral.mineralRockBaseHealth;
        rockBreakBar.value = currentMineral.currentRockHealth;

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
                itemImage.sprite = currentMineral.currentItem.PickableSprite;
                itemImage.color = Color.white;
                mineralsHealth.Add(selectedMineralControler.minerals[i], currentMineral.mineralsHealth[i]);
                selectedMineralControler.activeMinerals[selectedMineralControler.minerals[i]] = true;

                if (currentMineral.mineralsHealth[i] <= 0)
                {
                    itemImage.color = new Color(1, 1, 1, 0.2f);
                    selectedMineralControler.activeMinerals[selectedMineralControler.minerals[i]] = false;
                }
            }
            else
            {
                itemImage.enabled = false;
                itemImage.color = Color.white;
                selectedMineralControler.activeMinerals[selectedMineralControler.minerals[i]] = false;
            }
        }


        SetRandomPositionMinerals();
        selectedMineralControler.ChangeSelectedMineral();
        
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
                currentMineral.currentRockHealth -= (rockBreakSpeed * 0.5f) * Time.deltaTime;
        }
        else
        {
            //Romper aun mas la roca
            currentMineral.currentRockHealth -= (rockBreakSpeed * 2 * _usedLasers) * Time.deltaTime;
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
        

        InventoryManager.Instance.ChangeRunItemAmount(currentMineral.currentItem, 1);
        selectedMineralControler.selectedMineral.GetComponent<Image>().color = new Color(1,1,1, 0.2f);
        selectedMineralControler.activeMinerals[selectedMineralControler.selectedMineral] = false;
        if (selectedMineralControler.activeMinerals.ContainsValue(true))
            selectedMineralControler.MineralMined();
        else
            StopMining(true);//Parar de minar
        
    }

    private void BreakRock()
    {
        currentMineral.currentRockHealth -= rockBreakSpeed * Time.deltaTime;
        CheckIfRockCompletlyBroke();
    }
    private void CheckIfRockCompletlyBroke()
    {
        if (currentMineral.currentRockHealth <= 0)
        {
            StopMining(true);
        }
    }

    private void StopMining(bool _destroyRock)
    {
        selectedMineralControler.enabled = false;
        PlayerManager.Instance.player.ChangeState(PlayerController.State.IDLE);
        for (int i = 0; i < currentMineral.MaxItemsToReturn; i++)
        {
            currentMineral.mineralsHealth[i] = mineralsHealth[selectedMineralControler.minerals[i]];

        }


        if (_destroyRock)
        {
            currentMineral.gameObject.SetActive(false);
            CameraController.Instance.AddMediumTrauma();
            PlayerManager.Instance.player.ChangeState(PlayerController.State.MOVING);
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
        rockBreakBar.value = currentMineral.currentRockHealth;
        if (mineralsHealth.ContainsKey(selectedMineralControler.selectedMineral))
            selectedMineralControler.selectionIcon.fillAmount = mineralsHealth[selectedMineralControler.selectedMineral] / currentMineral.currentItem.BaseMineralHealth;        
    }
    #endregion


    #region Input
    private void LeftLaserAction(InputAction.CallbackContext obj)
    {
        leftLaserOn = obj.action.IsInProgress();
    }
    private void RightLaserAction(InputAction.CallbackContext obj)
    {
        rightLaserOn = obj.action.IsInProgress();
    }

    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }
    #endregion
}
