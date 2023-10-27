using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MineMinigameManager : MonoBehaviour
{
    [Space, SerializeField]
    private float laserChargeSpeed;
    [SerializeField]
    private float laserDischargeSpeed;

    [ SerializeField]
    private Vector2 neededSizes;
    
    [SerializeField]
    private Color correctEnergyColor = Color.blue;
    [SerializeField]
    private Color wrongEnergyColor = Color.grey;

    [Space, Header("Left Laser"), SerializeField]
    private MinigameBarController leftLaser;
    private bool chargingLeftLaser;
    [Space, Header("Right Laser"), SerializeField]
    private MinigameBarController rightLaser;
    private bool chargingRightLaser;

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
    
    void Start()
    {
        integrityValue = maxIntegrity;
        c_progressBarSlider.maxValue = maxIntegrity;

        leftLaser.SetCurrentEnergyLevel(50f);
        rightLaser.SetCurrentEnergyLevel(50f);

        GenerateRandomNeededEnergyLevels();
    }

    // Update is called once per frame
    void Update()
    {
        ManageInputs();
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

    
    private void ManageInputs() 
    {
        chargingLeftLaser = Input.GetButton("Fire1");
        chargingRightLaser = Input.GetButton("Fire2");
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
        CheckCurrentLaserEnergy(leftLaser);   
        CheckCurrentLaserEnergy(rightLaser);   
    }

    private void CheckCurrentLaserEnergy(MinigameBarController _currentLaser)
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
        }
        else
        {
            //No tiene la energia necesaria
            _currentLaser.SetCurrentEnergyPointerColor(wrongEnergyColor);
            _currentLaser.CorrectEnergy = false;
        }
    }

    private void GenerateRandomNeededEnergyLevels()
    {
        leftLaser.SetNeedEnergyLevel(Random.Range(10f, 90f), Random.Range(neededSizes.x, neededSizes.y));
        rightLaser.SetNeedEnergyLevel(Random.Range(10f, 90f), Random.Range(neededSizes.x, neededSizes.y));
    }

    private void CheckAdvanceProgress()
    {
        if (rightLaser.CorrectEnergy && leftLaser.CorrectEnergy)
        {
            //++
            progressValue += (progressSpeed * 2) * Time.deltaTime;
        }
        else if(rightLaser.CorrectEnergy || leftLaser.CorrectEnergy)
        {
            //+-
            progressValue += progressSpeed * Time.deltaTime;
            integrityValue -= breakSpeed * Time.deltaTime;
        }
        else
        {
            //--
            integrityValue -= (breakSpeed * 2) * Time.deltaTime;
        }

        c_progressBarSlider.value = progressValue;
        c_integrity.value = integrityValue;
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
        GetMinerals();

        progressValue = 0;
        integrityValue = 0;

        gameObject.SetActive(false);
    }

    private void GetMinerals()
    {
        float quarterIntegrity = maxIntegrity / 4;

        if (integrityValue >= quarterIntegrity * 3) //100% de minerales
        {
            Debug.Log("100%");
        }
        else if (integrityValue >= quarterIntegrity * 2) //75% de minerales
        {
            Debug.Log("75%");
        }
        else if (integrityValue >= quarterIntegrity) //50% de minerales
        {
            Debug.Log("50%");
        }
        else if (integrityValue > 0) //25% de minerales
        {
            Debug.Log("25%");

        }
        else // 0% de minerales
        {
            Debug.Log("0%");
        }
    }

}
