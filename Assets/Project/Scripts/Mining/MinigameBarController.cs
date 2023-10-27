using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameBarController : MonoBehaviour
{
    [SerializeField]
    private RectTransform c_currentEnergyLevelPointer;
    [SerializeField]
    private RectTransform c_needEnergyLevelPointer;

    private float currentEnergyValue;
    private float needEnergyValue;
    private float needEnergySize;
    private float neededEnergyOffset; ////Esto es el width del need energy dividido por 4

    private Slider c_miningBarSlider;
    private Image c_currentEnergyImage;

    [HideInInspector]
    public bool CorrectEnergy;

    private void Awake()
    {
        c_miningBarSlider = GetComponent<Slider>();
        c_currentEnergyImage = c_currentEnergyLevelPointer.GetComponent<Image>();
    }

    public void SetNeedEnergyLevel(float _nextEnergyNeed, float _nextEnergyNeedSize)
    {
        c_miningBarSlider.handleRect = c_needEnergyLevelPointer;
        needEnergyValue = _nextEnergyNeed;
        c_miningBarSlider.value = needEnergyValue;
        c_miningBarSlider.handleRect = c_currentEnergyLevelPointer;

        SetNeedEnergySize(_nextEnergyNeedSize);

    }
    public void SetCurrentEnergyLevel(float _nextCurrentEnergy)
    {
        currentEnergyValue = _nextCurrentEnergy;
        c_miningBarSlider.value = currentEnergyValue;
    }
    private void SetNeedEnergySize(float _nextEnergyNeedSize)
    {
        needEnergySize = _nextEnergyNeedSize;
        c_needEnergyLevelPointer.sizeDelta = new Vector2(needEnergySize, c_needEnergyLevelPointer.sizeDelta.y);
        c_needEnergyLevelPointer.position = new Vector3(0, 0, 0);
        c_needEnergyLevelPointer.anchoredPosition = new Vector3(0, 0, 0);
        //Cuando divides el tamanyo por 4 es perfecto pero a veces parece que sea injusto porque te pilla la parte de en medio del puntero de la energia actual
        //Al dividirlo entre 3 hasta que no sale el puntero entero no se marcara como que esta fuera
        neededEnergyOffset = needEnergySize / 3;
    }
    public void SetCurrentEnergyPointerColor(Color _currentColor)
    {
        c_currentEnergyImage.color = _currentColor;
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
