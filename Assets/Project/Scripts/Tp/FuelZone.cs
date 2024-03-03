using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelZone : MonoBehaviour
{
    [SerializeField]
    private float fuelIncrement;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.fuelConsume += fuelIncrement;
            PlayerManager.Instance.player.refillFuelParticles.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerManager.Instance.player.fuel >= PlayerManager.Instance.player.GetMaxFuel())
            {
                PlayerManager.Instance.player.refillFuelParticles.Stop();
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
        }
    }
}
