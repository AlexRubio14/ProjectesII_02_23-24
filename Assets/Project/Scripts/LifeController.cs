using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeController : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    private float health;

    [SerializeField]
    private TextMeshProUGUI c_textToPrint;

    private void Awake()
    {
        health = playerController.GetFuel();
        c_textToPrint.text = health.ToString("0");
        
    }
    


    // Update is called once per frame
    void Update()
    {
        health = playerController.GetFuel();
        c_textToPrint.text = health.ToString("0");
    }
}
