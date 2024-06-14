using System;
using System.Collections;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip leafCollisionClip;

    [SerializeField]
    private GamepadRumbleManager.Rumble touchGrassGamepadRumble;

    private GrassVelocityController grassVelocityController;

    private GameObject player;

    private Material material; 

    private Rigidbody2D playerRb2D;

    private bool easeInCoroutineRunning;
    private bool easeOutCoroutineRunning;

    private int externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    private float startingXVelocity;
    private float velocityLastFrame;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); 
        playerRb2D = player.GetComponent<Rigidbody2D>();
        grassVelocityController = GetComponentInParent<GrassVelocityController>();

        material = GetComponent<SpriteRenderer>().material;
        startingXVelocity = material.GetFloat(externalInfluence); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            if (!easeInCoroutineRunning && Mathf.Abs(playerRb2D.velocity.x) < Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRb2D.velocity.x * grassVelocityController.ExternalInfluenceStrength));
            }

            AudioManager.instance.Play2dOneShotSound(leafCollisionClip, "Leafs", 1, 0.5f, 1.5f);
            GamepadRumbleManager.Instance.AddRumble(touchGrassGamepadRumble);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            StartCoroutine(EaseOut());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            if (Mathf.Abs(velocityLastFrame) > MathF.Abs(grassVelocityController.VelocityThreshold) &&
                MathF.Abs(playerRb2D.velocity.x) < Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            }
            else if (Mathf.Abs(velocityLastFrame) < Mathf.Abs(grassVelocityController.VelocityThreshold) &&
                Mathf.Abs(playerRb2D.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRb2D.velocity.x * grassVelocityController.ExternalInfluenceStrength));
            }
            else if (!easeInCoroutineRunning && !easeOutCoroutineRunning &&
                MathF.Abs(playerRb2D.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                grassVelocityController.InfluenceGrass(material, playerRb2D.velocity.x * grassVelocityController.ExternalInfluenceStrength);
            }

            velocityLastFrame = playerRb2D.velocity.x;
        }
    }
    private IEnumerator EaseIn(float XVelocity)
    {
        easeInCoroutineRunning = true;
        float elapsedTime = 0f;

        if (XVelocity > 0)
            XVelocity += 2;

        while (elapsedTime < grassVelocityController.EaseInTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(startingXVelocity, XVelocity, (elapsedTime / grassVelocityController.EaseInTime));
            grassVelocityController.InfluenceGrass(material, lerpedAmount);

            yield return null;
        }

        easeInCoroutineRunning = false;
    }

    private IEnumerator EaseOut()
    {
        easeOutCoroutineRunning = true;
        float currentXInfluence = material.GetFloat(externalInfluence);

        float elapsedTime = 0f; 
        while(elapsedTime < grassVelocityController.EaseOutTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(currentXInfluence, startingXVelocity, (elapsedTime/ grassVelocityController.EaseOutTime));
            grassVelocityController.InfluenceGrass(material, lerpedAmount); 

            yield return null;  
        }

        easeOutCoroutineRunning = false;
    }
}
