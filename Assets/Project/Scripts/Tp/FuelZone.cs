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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.fuelConsume -= fuelIncrement;
        }
    }
}
