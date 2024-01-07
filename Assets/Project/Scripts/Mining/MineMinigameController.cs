using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MineMinigameController : MonoBehaviour
{
    [Header("Input"), SerializeField]
    private InputActionReference chargeRightLaserAction;
    [SerializeField]
    private InputActionReference chargeLeftLaserAction;

    [Space, Header(""), SerializeField]
    private float laserChargeSpeed;
    [SerializeField]
    private float laserDischargeSpeed;

    [SerializeField]
    private Vector2 neededSizes;
    
    [SerializeField]
    private Color correctEnergyColor = Color.blue;
    [SerializeField]
    private Color wrongEnergyColor = Color.grey;

    [Space, SerializeField]
    private float laserSliderSpeed;

    [Space, Header("Left Laser"), SerializeField]
    private MinigameBarController leftLaser;
    public bool chargingLeftLaser { set; get; }
    [SerializeField]
    private Slider leftLaserSlider;
    [SerializeField]
    private ParticleSystem leftLaserParticles;

    [Space, Header("Right Laser"), SerializeField]
    private MinigameBarController rightLaser;
    public bool chargingRightLaser { set; get; }
    [SerializeField]
    private Slider rightLaserSlider;
    [SerializeField]
    private ParticleSystem rightLaserParticles;

    [Space, Header("Mine"), SerializeField]
    private Slider c_progressBarSlider;
    [SerializeField]
    private Slider c_integrity;
    [SerializeField]
    private float progressSpeed;
    [SerializeField]
    private float breakSpeed;
    [SerializeField]
    private float maxIntegrity;
    private float progressValue;
    private float integrityValue;

    [Space, Header("Multiplier"), SerializeField]
    private float maxMultiplierSpeed;
    private float currentMultiplierSpeed;
    [SerializeField]
    private float multiplierUpSpeed;
    [SerializeField]
    private float multipliersDownSpeed;
    [Header("Multiplier Shake"), SerializeField]
    private float shakeMagnitude;
    [SerializeField]
    private GameObject oreBox;
    private Vector2 startPos;
    [Header("Multiplier Lasers"), SerializeField]
    private GameObject rightLaserPivot;
    [SerializeField]
    private GameObject leftLaserPivot;
    [SerializeField]
    private float maxAngleRotation;
    private Quaternion starterRotationRightLaser;
    private Quaternion starterRotationLeftLaser;
    private Quaternion endRotationRightLaser;
    private Quaternion endRotationLeftLaser;

    [Space, Header("Minerals"), SerializeField]
    private GameObject c_pickableItemPrefab;
    [SerializeField]
    private float maxThrowSpeed;

    private MineralController c_miningItem;
    [SerializeField]
    private TextMeshProUGUI[] percentText;
    [SerializeField]
    private Image[] mineralTypeImages;

    [Space, Header("Audio"), SerializeField]
    private AudioClip miningAudio;
    private AudioSource leftLaserSource;
    private AudioSource rightLaserSource;

    private void Start()
    {
        startPos = oreBox.transform.localPosition;
        starterRotationRightLaser = rightLaserPivot.transform.rotation;
        starterRotationLeftLaser = leftLaserPivot.transform.rotation;

        endRotationRightLaser = rightLaserPivot.transform.rotation * Quaternion.Euler(0, 0, maxAngleRotation);
        endRotationLeftLaser = leftLaserPivot.transform.rotation * Quaternion.Euler(0, 0, -maxAngleRotation);
    }

    void OnEnable()
    {
        integrityValue = maxIntegrity;
        c_progressBarSlider.maxValue = maxIntegrity;

        currentMultiplierSpeed = 1;

        leftLaser.SetCurrentEnergyLevel(50f);
        rightLaser.SetCurrentEnergyLevel(50f);

        SetupQuantityText();
        SetupMineralTypeImages();


        StartCoroutine(GenerateRandomNeededEnergyLevels());


        chargeRightLaserAction.action.started += ChargeRightLaserAction;
        chargeRightLaserAction.action.canceled += ChargeRightLaserAction;

        chargeLeftLaserAction.action.started += ChargeLeftLaserAction;
        chargeLeftLaserAction.action.canceled += ChargeLeftLaserAction;

    }
    private void OnDisable()
    {
        chargeRightLaserAction.action.started -= ChargeRightLaserAction;
        chargeRightLaserAction.action.canceled -= ChargeRightLaserAction;

        chargeLeftLaserAction.action.started -= ChargeLeftLaserAction;
        chargeLeftLaserAction.action.canceled -= ChargeLeftLaserAction;
    }

    // Update is called once per frame
    void Update()
    {
        SetLasersValue();
        CheckLasersEnergy();
        CheckAdvanceProgress();
        CheckProgressEnded();

        if (Input.GetKeyDown(KeyCode.M))
        {
            //Acabar de minar
            EndMining();
        }
    }

    private void SetLasersValue() 
    {
        ChangeCurrentLaser(leftLaser, chargingLeftLaser);
        ChangeCurrentLaser(rightLaser, chargingRightLaser);
    }
    private void ChangeCurrentLaser(MinigameBarController _currentLaser, bool _chargingCurrentLaser)
    {
        float currentEnergy = _currentLaser.GetCurrentEnergy();

        if (_chargingCurrentLaser)
        {
            currentEnergy += laserChargeSpeed * Time.deltaTime;
        }
        else
        {
            currentEnergy -= laserDischargeSpeed * Time.deltaTime;
        }
        currentEnergy = Mathf.Clamp(currentEnergy, 0, 100);
        _currentLaser.SetCurrentEnergyLevel(currentEnergy);

    }

    private void CheckLasersEnergy()
    {
        CheckCurrentLaserEnergy(leftLaser, leftLaserSlider, leftLaserParticles);   
        CheckCurrentLaserEnergy(rightLaser, rightLaserSlider, rightLaserParticles);   
    }

    private void CheckCurrentLaserEnergy(MinigameBarController _currentLaser, Slider _currentLaserSlider, ParticleSystem _currentParticles)
    {
        float currentEnergy = _currentLaser.GetCurrentEnergy();
        float currentNeededEnergy = _currentLaser.GetNeedEnergy();
        float currentOffset = _currentLaser.GetEnergyOffset() / 2;

        if (currentEnergy >= currentNeededEnergy - currentOffset &&
            currentEnergy <= currentNeededEnergy + currentOffset)
        {
            //Tiene la energia necesaria
            _currentLaser.SetCurrentEnergyPointerColor(correctEnergyColor);
            _currentLaser.CorrectEnergy = true;

            _currentLaserSlider.value += Time.deltaTime * laserSliderSpeed;
        }
        else
        {
            //No tiene la energia necesaria
            _currentLaser.SetCurrentEnergyPointerColor(wrongEnergyColor);
            _currentLaser.CorrectEnergy = false;
            _currentLaserSlider.value -= Time.deltaTime * laserSliderSpeed;

        }

        if (_currentLaserSlider.value >= 0.9f && _currentParticles.isStopped)
        {
            _currentParticles.Play(true);
        }
        else if (_currentLaserSlider.value < 0.9f && _currentParticles.isPlaying)
        {
            _currentParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }


    }

    private IEnumerator GenerateRandomNeededEnergyLevels()
    {

        yield return new WaitForEndOfFrame();
        leftLaser.SetNeedEnergyLevel(Random.Range(10f, 90f), Random.Range(neededSizes.x, neededSizes.y));
        rightLaser.SetNeedEnergyLevel(Random.Range(10f, 90f), Random.Range(neededSizes.x, neededSizes.y));
    }

    private void CheckAdvanceProgress()
    {
        if (rightLaser.CorrectEnergy && leftLaser.CorrectEnergy)
        {
            //++
            currentMultiplierSpeed += multiplierUpSpeed * Time.deltaTime;
            currentMultiplierSpeed = Mathf.Clamp(currentMultiplierSpeed, 1, maxMultiplierSpeed);
            progressValue += (progressSpeed * 2 * currentMultiplierSpeed) * Time.deltaTime;

        }
        else if(rightLaser.CorrectEnergy || leftLaser.CorrectEnergy)
        {
            //+-
            currentMultiplierSpeed -= multipliersDownSpeed * Time.deltaTime;
            currentMultiplierSpeed = Mathf.Clamp(currentMultiplierSpeed, 1, maxMultiplierSpeed);
            progressValue += (progressSpeed * currentMultiplierSpeed) * Time.deltaTime;
            integrityValue -= breakSpeed * Time.deltaTime;
        }
        else
        {
            //--
            currentMultiplierSpeed -= multipliersDownSpeed * Time.deltaTime;
            currentMultiplierSpeed = Mathf.Clamp(currentMultiplierSpeed, 1, maxMultiplierSpeed);
            integrityValue -= (breakSpeed * 2) * Time.deltaTime;
        }

        c_progressBarSlider.value = progressValue;
        c_integrity.value = integrityValue;

        MultiplierFeedback();

    }

    private void MultiplierFeedback()
    {
        //Hacemos que tiemble con un random, este segun mas alto el multiplicador mas temblara
        oreBox.transform.localPosition = startPos + (new Vector2(
            Random.Range(-shakeMagnitude, shakeMagnitude) * (currentMultiplierSpeed - 1),
            Random.Range(-shakeMagnitude, shakeMagnitude) * (currentMultiplierSpeed - 1)));


        //Ponemos los rayos en la posicion que les toca
        float process = (currentMultiplierSpeed - 1) / 3;
        rightLaserPivot.transform.rotation = Quaternion.Lerp(starterRotationRightLaser, endRotationRightLaser, process);
        leftLaserPivot.transform.rotation = Quaternion.Lerp(starterRotationLeftLaser, endRotationLeftLaser, process);


    }

    private void CheckProgressEnded()
    {
        if (progressValue >= 100 || integrityValue <= 0)
        {
            //Acabar de minar
            EndMining();
        }
    }

    private void EndMining()
    {
        ThrowMinerals(CalculateMinerals(integrityValue));

        c_miningItem.gameObject.SetActive(false);
        CameraController.Instance.AddMediumTrauma();
        progressValue = 0;
        integrityValue = 0;

        PlayerManager.Instance.player.ChangeState(PlayerController.State.MOVING);

        gameObject.SetActive(false);
    }

    private short CalculateMinerals(float _currentIntegrity)
    {
        float quarterIntegrity = maxIntegrity / 4;

        short itemsToReturn; 
        if (_currentIntegrity >= quarterIntegrity * 3) //100% de minerales
        {
            itemsToReturn = c_miningItem.MaxItemsToReturn;
        }
        else if (_currentIntegrity >= quarterIntegrity * 2) //75% de minerales
        {
            itemsToReturn = (short)Mathf.CeilToInt(c_miningItem.MaxItemsToReturn * 0.75f);
        }
        else if (_currentIntegrity >= quarterIntegrity) //50% de minerales
        {
            itemsToReturn = (short)Mathf.CeilToInt(c_miningItem.MaxItemsToReturn * 0.5f);
        }
        else if (_currentIntegrity > 0) //25% de minerales
        {
            itemsToReturn = (short)Mathf.CeilToInt(c_miningItem.MaxItemsToReturn * 0.25f);
        }
        else // 0% de minerales
        {
            itemsToReturn = 0;
        }


        return itemsToReturn;

    }

    private void ThrowMinerals(float _itemsToReturn)
    {
        for (int i = 0; i < _itemsToReturn; i++)
        {
            PickableItemController currItem = Instantiate(c_pickableItemPrefab, c_miningItem.transform.position, Quaternion.identity).GetComponent<PickableItemController>();

            currItem.c_currentItem = c_miningItem.c_currentItem;

            float randomX = Random.Range(-1, 2);
            float randomY = Random.Range(-1, 2);
            Vector2 randomDir = new Vector2(randomX, randomY);

            randomDir.Normalize();

            float throwSpeed = Random.Range(0, maxThrowSpeed);
            currItem.ImpulseItem(randomDir, throwSpeed);
            currItem.transform.up = randomDir;
        }
    }
    public void SetMiningObject(MineralController _mineral)
    {
        c_miningItem = _mineral;
    }

    private void SetupQuantityText()
    {
        for (int i = 0; i < percentText.Length; i++)
        {
            percentText[i].text =  "x" + CalculateMinerals(100 - ( 26 * i)).ToString();
        }
    }
    private void SetupMineralTypeImages()
    {
        for (int i = 0; i < mineralTypeImages.Length; i++)
        {
            mineralTypeImages[i].sprite = c_miningItem.c_currentItem.c_PickableSprite;
        }
    }


    #region Input 
    private void ChargeRightLaserAction(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            StartCoroutine(AudioManager._instance.FadeOutSFXLoop(rightLaserSource));
            chargingRightLaser = false;
        }
        else
        {
            rightLaserSource = AudioManager._instance.Play2dLoop(miningAudio, "Mining");
            chargingRightLaser = true;
        }
    }
    private void ChargeLeftLaserAction(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            StartCoroutine(AudioManager._instance.FadeOutSFXLoop(leftLaserSource));
            chargingLeftLaser = false;
        }
        else
        {
            leftLaserSource = AudioManager._instance.Play2dLoop(miningAudio, "Mining");
            chargingLeftLaser = true;
        }
    }
    #endregion

}
