using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelZone : MonoBehaviour
{
    [SerializeField]
    private float fuelIncrement;

    [SerializeField]
    private AudioClip healingClip;
    private AudioSource healingSource;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.fuelConsume += fuelIncrement;
            PlayerManager.Instance.player.refillFuelParticles.Play();
            healingSource = AudioManager._instance.Play2dLoop(healingClip, "Teleport");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerManager.Instance.player.fuel >= PlayerManager.Instance.player.GetMaxFuel())
            {
                PlayerManager.Instance.player.refillFuelParticles.Stop();
                AudioManager._instance.StopLoopSound(healingSource);
            }
            else if (PlayerManager.Instance.player.refillFuelParticles.isStopped)
            {
                PlayerManager.Instance.player.refillFuelParticles.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.fuelConsume -= fuelIncrement;
            PlayerManager.Instance.player.refillFuelParticles.Stop();
            AudioManager._instance.StopLoopSound(healingSource);
        }
    }
}
