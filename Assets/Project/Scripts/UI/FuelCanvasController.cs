using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using Unity.VisualScripting;

public class FuelCanvasController : MonoBehaviour
{
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
    [Space, SerializeField]
    private float blinkSpeed;
    private float colorFadeProgress;
    private bool turningRedUI;
    [SerializeField]
    private float backgroundFuelInfluence;

    [Space, SerializeField]
    private float shakeMagnitude;
    private Vector2[] starterPos;
    
    private ImageFloatEffect sliderFloatEffect;
    [Space, SerializeField]
    private float maxFloatSpeed;
    [SerializeField]
    private float baseFloatSpeed;

    [Space, SerializeField]
    private Vector2 dangerBorderAlpha;
    [SerializeField]
    private float dangerBorderSpeed;
    private float dangerBorderProcess;
    private bool turningRedBorder = false;
    [SerializeField]
    private float borderFuelInfluence;

    [Space, Header("Hitted"), SerializeField]
    private float hitShakeForce;
    private float hitShakeProcess;
    [SerializeField]
    private float hitShakeReduction;
    private bool hitted = false;

    [Space, SerializeField]
    private float hitBorderMaxAlpha;
    [SerializeField]
    private Vector2 hitBorderEnableSpeed;
    private float hitBorderProcess;
    private bool hitBorderEnable;

    [SerializeField]
    private Image lowFuelBorder;
    private bool canFuelBorder;

    private void Awake()
    {
        sliderFloatEffect = GetComponentInChildren<ImageFloatEffect>();
    }

    private void Start()
    {

        playerController = PlayerManager.Instance.player;

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

    private void OnEnable()
    {
        PlayerManager.Instance.player.OnHit += Hitted;
    }

    private void OnDisable()
    {
        playerController.OnHit -= Hitted;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        float currFuel = playerController.GetFuel();
        currentFuel.text = currFuel.ToString("0.0");
        fuelSlider.value = currFuel;
        totalConsumePerSecond.text = playerController.fuelConsume.ToString("0.0");

        if (currFuel <= 0)
        {
            RestoreDefaultValues();
            return;
        }

        LowFuelFeedback(currFuel);
        BorderHitEffect();
    }

    private void LowFuelFeedback(float _currFuel)
    {
        if (playerController.fuelConsume > 0)
        {
            RestoreDefaultValues();
            sliderFloatEffect.canFloat = true;
            sliderFloatEffect.speed = baseFloatSpeed;
        }
        else if (_currFuel <= fuelPercent && hitShakeProcess < shakeMagnitude)
        {
            float maxFuelInfluence = 6;
            float minFuelInfluence = 1.1f;
            sliderFloatEffect.canFloat = true;
            sliderFloatEffect.speed = Mathf.Clamp(
                baseFloatSpeed / Mathf.Clamp(_currFuel / backgroundFuelInfluence, minFuelInfluence, maxFuelInfluence),
                0,
                maxFloatSpeed);

            ShakeCanvas(shakeMagnitude);
            ChangeBackgroundColor(_currFuel);
            BorderColorLowFuel(_currFuel);
        }
        else if (hitted) 
        {
            float shakeValue = Mathf.Lerp(0, hitShakeForce, hitShakeProcess);
            ShakeCanvas(shakeValue);

            hitShakeProcess -= Time.deltaTime * hitShakeReduction;

            if (hitShakeProcess <= 0)
            {
                hitShakeProcess = 0;
                hitted = false;
            }
        }
        else
        {
            RestoreDefaultValues();
        }
    }
    private void ShakeCanvas(float _shakeMagnitude)
    {
        float shakeX = Random.Range(-_shakeMagnitude, _shakeMagnitude);
        float shakeY = Random.Range(-_shakeMagnitude, _shakeMagnitude); 
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.parent.transform.localPosition = 
                starterPos[i] + new Vector2(
                    shakeX,
                    shakeY
                    );
        }
    }
    private void ChangeBackgroundColor(float _currFuel)
    {
        if (_currFuel <= 0)
        {
            foreach (Image item in backgrounds)
                item.color = warningColor;

            sliderFloatEffect.canFloat = false;

            return;
        }

        float maxFuelInfluence = 6;
        float minFuelInfluence = 1.1f;

        // A la hora de hacer que el fuel actual influya en en la velocidad la limitaremos a un maximo que puede afectar
        if (turningRedUI)
            colorFadeProgress = Mathf.Clamp(
                colorFadeProgress + blinkSpeed / Mathf.Clamp(_currFuel / backgroundFuelInfluence, minFuelInfluence, maxFuelInfluence) * Time.deltaTime,
                0,
                1);
        else
            colorFadeProgress = Mathf.Clamp(
                colorFadeProgress - blinkSpeed / Mathf.Clamp(_currFuel / backgroundFuelInfluence, minFuelInfluence, maxFuelInfluence) * Time.deltaTime,
                0,
                1);

        foreach (Image item in backgrounds)
            item.color = Color.Lerp(starterColor, warningColor, colorFadeProgress);
        

        if (colorFadeProgress >= 1)
            turningRedUI = false;
        else if (colorFadeProgress <= 0)
            turningRedUI = true;

    }
    private void BorderColorLowFuel(float _currFuel)
    {
        if (_currFuel <= 0)
        {
            lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, 0);
            return;
        }
        // A la hora de hacer que el fuel actual influya en en la velocidad la limitaremos a un maximo que puede afectar
        float maxFuelInfluence = 6f;
        float minFuelInfluence = 1.5f;

        if (turningRedBorder)
            dangerBorderProcess = Mathf.Clamp(
                dangerBorderProcess + dangerBorderSpeed / Mathf.Clamp(_currFuel / borderFuelInfluence, minFuelInfluence, maxFuelInfluence) * Time.deltaTime,
                dangerBorderAlpha[0], 
                dangerBorderAlpha[1]);
        else
            dangerBorderProcess = Mathf.Clamp(
                dangerBorderProcess - dangerBorderSpeed / Mathf.Clamp(_currFuel / borderFuelInfluence, minFuelInfluence, maxFuelInfluence) * Time.deltaTime,
                dangerBorderAlpha[0],
                dangerBorderAlpha[1]);

        lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, dangerBorderProcess); 

        if (dangerBorderProcess >= dangerBorderAlpha[1])
            turningRedBorder = false;
        else if (dangerBorderProcess <= dangerBorderAlpha[0])
            turningRedBorder = true;


    }
    private void Hitted()
    {
        hitted = true;
        hitShakeProcess = hitShakeForce;
        canFuelBorder = true;
        hitBorderEnable = true;
        lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, 0);
    }

    private void BorderHitEffect()
    {
        if (!canFuelBorder)
            return;
        
        if (hitBorderEnable) 
        {
            hitBorderProcess = Mathf.Clamp(hitBorderProcess + hitBorderEnableSpeed[0] * Time.deltaTime, 0 , hitBorderMaxAlpha);
            lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, hitBorderProcess);

            if (hitBorderProcess >= hitBorderMaxAlpha)
                hitBorderEnable = false;

        }
        else
        {
            hitBorderProcess = Mathf.Clamp(hitBorderProcess - hitBorderEnableSpeed[1] * Time.deltaTime, 0, hitBorderMaxAlpha);
            lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, hitBorderProcess);

            if (hitBorderProcess <= 0)
            {
                canFuelBorder = false;
                hitBorderEnable = true;
                hitBorderProcess = 0;
                lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, 0);
                return;
            }
            else if (hitBorderProcess <= dangerBorderProcess)
            {
                canFuelBorder = false;
                hitBorderEnable = true;
                hitBorderProcess = 0;
            }
        }

    }

    private void RestoreDefaultValues()
    {
        //Reseteamos posiciones
        for (int i = 0; i < backgrounds.Length; i++)
            backgrounds[i].transform.parent.transform.localPosition = starterPos[i];

        //Reseteamos color
        foreach (Image item in backgrounds)
            item.color = starterColor;
        //Reseteamos Flotacion
        sliderFloatEffect.canFloat = false;
        lowFuelBorder.color = new Color(lowFuelBorder.color.r, lowFuelBorder.color.g, lowFuelBorder.color.b, 0);
        dangerBorderProcess = 0;
    }
}
