using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelCanvasController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [Header("Fuel"), SerializeField]
    private TextMeshProUGUI currentFuel;
    [SerializeField]
    private TextMeshProUGUI starterFuel;
    [SerializeField]
    private TextMeshProUGUI totalConsumePerSecond;
    [SerializeField]
    private Slider fuelSlider;

    [Space, Header("Danger"), SerializeField, Range(0, 100)]
    private int dangerFuelPercent;
    private float fuelPercent;
    [SerializeField]
    private Image[] backgrounds;
    [SerializeField]
    private Color warningColor;
    private Color starterColor;
    [SerializeField]
    private float blinkSpeed;
    private float colorFadeProgress;
    private bool turningRed;

    [SerializeField]
    private float shakeMagnitude;
    private Vector2[] starterPos;



    private void Start()
    {
        float maxFuel = playerController.GetMaxFuel();
        starterFuel.text = maxFuel.ToString("0") + "L";
        fuelSlider.maxValue = maxFuel;

        fuelPercent = maxFuel * dangerFuelPercent / 100;

        starterColor = backgrounds[0].color;

        starterPos = new Vector2[backgrounds.Length];

        for (int i = 0; i < starterPos.Length; i++)
        {
            starterPos[i] = backgrounds[i].transform.parent.transform.localPosition; 
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float currFuel = playerController.GetFuel();
        currentFuel.text = currFuel.ToString("0.0");
        fuelSlider.value = currFuel;

        ShowLowFuel(currFuel);

    }

    private void ShowLowFuel(float _currFuel)
    {
        if (_currFuel <= fuelPercent)
        {
            ShakeCanvas();
            ChangeBackgroundColor(_currFuel);
        }
    }

    private void ShakeCanvas()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.parent.transform.localPosition = 
                starterPos[i] + new Vector2(
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    Random.Range(-shakeMagnitude, shakeMagnitude)
                    );
        }
    }
    private void ChangeBackgroundColor(float _currFuel)
    {
        if (_currFuel <= 0)
        {
            foreach (Image item in backgrounds)
            {
                item.color = warningColor;
            }
            return;
        }

        if (turningRed)
        {
            colorFadeProgress += (blinkSpeed / (_currFuel / 4)) * Time.deltaTime;
        }
        else
        {
            colorFadeProgress -= (blinkSpeed / (_currFuel / 4)) * Time.deltaTime;
        }

        foreach (Image item in backgrounds)
        {
            item.color = Color.Lerp(starterColor, warningColor, colorFadeProgress);
        }

        if (colorFadeProgress >= 1)
        {
            turningRed = false;
            colorFadeProgress = 1;
        }
        else if (colorFadeProgress <= 0)
        {
            turningRed = true;
            colorFadeProgress = 0;
        }

    }

}
