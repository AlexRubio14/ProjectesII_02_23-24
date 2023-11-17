using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FuelCanvasController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    private float health;

    [SerializeField]
    private TextMeshProUGUI c_textToPrint;

    private void Awake()
    {
        health = playerController.GetFuel();
        c_textToPrint.text = " Fuel: " + health.ToString("0");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        health = playerController.GetFuel();
        c_textToPrint.text = " Fuel: " + health.ToString("0");
    }
}
