using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Electro : MonoBehaviour
{
    [SerializeField]
    private float delaySwitch = 0.2f;

    [SerializeField]
    private int bucleCount = 4;
    private int count = 0;

    [SerializeField]
    private Light2D spotLight;

    [SerializeField]
    private float minRangeLight = 2.0f; 
    private float maxRangeLight;

    private void Start()
    {
        maxRangeLight = spotLight.pointLightOuterRadius; 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count = 0; 
            InvokeRepeating("SwitchLight", 0.0001f, delaySwitch);
        }
    }

    private void SwitchLight()
    {
        if(spotLight.pointLightOuterRadius == maxRangeLight)
            spotLight.pointLightOuterRadius = minRangeLight;
        else
            spotLight.pointLightOuterRadius = maxRangeLight;

        if (count > bucleCount)
        {
            CancelInvoke();
            if (spotLight.pointLightOuterRadius == maxRangeLight)
                spotLight.pointLightOuterRadius = minRangeLight;
            else
                spotLight.pointLightOuterRadius = maxRangeLight;
        }
        count++;
    }
}
