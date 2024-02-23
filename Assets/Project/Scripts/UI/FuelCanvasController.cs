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
    private bool turningRed;
    [SerializeField]
    private float shakeMagnitude;
    private Vector2[] starterPos;
    
    private ImageFloatEffect sliderFloatEffect;
    [Space, SerializeField]
    private float maxFloatSpeed;
    [SerializeField]
    private float baseFloatSpeed;

    [Space, SerializeField]
    private Vector2 dangerVignetteIntensity;
    [SerializeField]
    private float dangerVignetteSpeed;
    private float dangerVignetteProcess;


    [Space, Header("Hitted"), SerializeField]
    private float hitShakeForce;
    private float hitShakeProcess;
    [SerializeField]
    private float hitShakeReduction;
    private bool hitted = false;
    [Space, SerializeField]
    private float hitVignetteMaxIntensity;
    [SerializeField]
    private Vector2 hitVignetteEnableSpeed;
    private float hitVignetteProcess;
    private bool hitVignetteEnable;

    [Space, Header("PostProces"), SerializeField]
    private Volume postProcess;
    private Vignette vignetteEffect;
    private bool canDisplayVignetteFX;

    private void Awake()
    {
        sliderFloatEffect = GetComponentInChildren<ImageFloatEffect>();
        postProcess.profile.TryGet(out vignetteEffect);
        vignetteEffect.active = true;
        vignetteEffect.intensity.value = 0;
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

        LowFuelFeedback(currFuel);
        VignetteHitEffect();
    }

    private void LowFuelFeedback(float _currFuel)
    {
        if (_currFuel <= fuelPercent && hitShakeProcess < shakeMagnitude)
        {
            sliderFloatEffect.canFloat = true;
            sliderFloatEffect.speed = Mathf.Clamp(baseFloatSpeed / (_currFuel / 4), 0, maxFloatSpeed);

            ShakeCanvas(shakeMagnitude);
            ChangeBackgroundColor(_currFuel);
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

            return;
        }

        if (turningRed)
            colorFadeProgress = Mathf.Clamp(colorFadeProgress + (blinkSpeed / (_currFuel / 4)) * Time.deltaTime, 0, 1);
        else
            colorFadeProgress = Mathf.Clamp(colorFadeProgress - (blinkSpeed / (_currFuel / 4)) * Time.deltaTime, 0, 1);

        foreach (Image item in backgrounds)
            item.color = Color.Lerp(starterColor, warningColor, colorFadeProgress);
        

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
    private void Hitted()
    {
        hitted = true;
        hitShakeProcess = hitShakeForce;
        canDisplayVignetteFX = true;
        hitVignetteEnable = true;
        vignetteEffect.intensity.value = 0;
    }

    private void VignetteHitEffect()
    {
        if (!canDisplayVignetteFX)
            return;
        
        if (hitVignetteEnable) 
        {
            hitVignetteProcess = Mathf.Clamp(hitVignetteProcess + hitVignetteEnableSpeed[0] * Time.deltaTime, 0 , hitVignetteMaxIntensity);
            vignetteEffect.intensity.value = hitVignetteProcess;

            if (hitVignetteProcess >= hitVignetteMaxIntensity)
                hitVignetteEnable = false;

        }
        else
        {
            hitVignetteProcess = Mathf.Clamp(hitVignetteProcess - hitVignetteEnableSpeed[1] * Time.deltaTime, 0, hitVignetteMaxIntensity);
            vignetteEffect.intensity.value = hitVignetteProcess;

            if (hitVignetteProcess <= 0)
            {
                canDisplayVignetteFX = false;
                hitVignetteEnable = true;
                hitVignetteProcess = 0;
                vignetteEffect.intensity.value = 0;
                return;
            }
            //else if (hitVignetteProcess <= )
            //{

            //}

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

    }

}
