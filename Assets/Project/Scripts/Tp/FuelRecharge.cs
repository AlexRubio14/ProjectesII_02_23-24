using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelRecharge : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.RefillFuel();
        }
    }
}
