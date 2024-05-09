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

    private Light2D spotLight;

    [SerializeField]
    private float minRangeLight = 2.0f; 
    private float maxRangeLight;

    [SerializeField]
    private Vector2 exitDirection;

    private bool lightOn;
    private Rigidbody2D playerRb2d;

    [Space, SerializeField]
    private AudioClip powerOnClip;
    [SerializeField]
    private AudioClip powerOffClip;

    private void Start()
    {
        spotLight = PlayerManager.Instance.player.GetComponentInChildren<Light2D>();
        playerRb2d = PlayerManager.Instance.player.GetComponent<Rigidbody2D>();
        maxRangeLight = spotLight.pointLightOuterRadius;
    }
    private void SwitchLight()
    {
        if (spotLight.pointLightOuterRadius == maxRangeLight)
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count = 0;

            if (Vector2.Dot(exitDirection, playerRb2d.velocity) >= 0)
            {
                //Sale
                lightOn = true;
                AudioManager.instance.Play2dOneShotSound(powerOnClip, "Electro");
            }
            else
            {
                //Entra
                lightOn = false;
                AudioManager.instance.Play2dOneShotSound(powerOffClip, "Electro");
            }

            if (lightOn && spotLight.pointLightOuterRadius != maxRangeLight ||
                !lightOn && spotLight.pointLightOuterRadius == maxRangeLight)
            {
                InvokeRepeating("SwitchLight", 0.0001f, delaySwitch);
            }

        }
    }


    private void OnDrawGizmosSelected()
    {
        exitDirection.Normalize();

        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(exitDirection * 3));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - (Vector3)(exitDirection * 3));
    }
}
