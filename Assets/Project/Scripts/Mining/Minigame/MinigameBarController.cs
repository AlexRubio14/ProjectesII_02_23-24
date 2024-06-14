using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameBarController : MonoBehaviour
{
    [SerializeField]
    private RectTransform currentEnergyLevelPointer;
    [SerializeField]
    private RectTransform needEnergyLevelPointer;

    private float currentEnergyValue;
    private float needEnergyValue;
    private float needEnergySize;
    private float neededEnergyOffset; ////Esto es el width del need energy dividido por 4
    private float maxEnergyDistanceAtStart = 25;


    private Slider miningBarSlider;
    private Image currentEnergyImage;

    [HideInInspector]
    public bool CorrectEnergy;

    private void Awake()
    {
        miningBarSlider = GetComponent<Slider>();
        currentEnergyImage = currentEnergyLevelPointer.GetComponent<Image>();
    }

    public void SetNeedEnergyLevel(float _nextEnergyNeed, float _nextEnergyNeedSize)
    {
        miningBarSlider.handleRect = needEnergyLevelPointer;
        needEnergyValue = _nextEnergyNeed;
        miningBarSlider.value = needEnergyValue;
        miningBarSlider.handleRect = currentEnergyLevelPointer;

        SetNeedEnergySize(_nextEnergyNeedSize);


        if (currentEnergyValue > needEnergyValue + maxEnergyDistanceAtStart)
            SetCurrentEnergyLevel(needEnergyValue + maxEnergyDistanceAtStart);
        else if (currentEnergyValue < needEnergyValue - maxEnergyDistanceAtStart)
            SetCurrentEnergyLevel(needEnergyValue - maxEnergyDistanceAtStart);

    }
    public void SetCurrentEnergyLevel(float _nextCurrentEnergy)
    {
        currentEnergyValue = _nextCurrentEnergy;
        if(miningBarSlider)
            miningBarSlider.value = currentEnergyValue;
    }
    private void SetNeedEnergySize(float _nextEnergyNeedSize)
    {
        needEnergySize = _nextEnergyNeedSize;
        needEnergyLevelPointer.sizeDelta = new Vector2(needEnergySize, needEnergyLevelPointer.sizeDelta.y);
        needEnergyLevelPointer.position = new Vector3(0, 0, 0);
        needEnergyLevelPointer.anchoredPosition = new Vector3(0, 0, 0);
        //Cuando divides el tamanyo por 4 es perfecto pero a veces parece que sea injusto porque te pilla la parte de en medio del puntero de la energia actual
        //Al dividirlo entre 3 hasta que no sale el puntero entero no se marcara como que esta fuera
        neededEnergyOffset = needEnergySize / 3;
    }
    public void SetCurrentEnergyPointerColor(Color _currentColor)
    {
        currentEnergyImage.color = _currentColor;
    }

    public float GetEnergyOffset()
    {
        return neededEnergyOffset;
    }
    public float GetNeedEnergy()
    {
        return needEnergyValue;
    }
    public float GetCurrentEnergy()
    {
        return currentEnergyValue;
    }



}
