using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    private int websInPlayer;

    public float webDecrease { get; private set; }

    [SerializeField]
    private float timeToEraseWeb;
    private float currentWebTime;

    [Space, Header("Speed Decrease"), SerializeField]
    private float speedDecreaseWithOneWeb;
    [SerializeField]
    private float speedDecreaseWithTwoWebs;
    [SerializeField]
    private float speedDecreaseWithThreeWebs;


    private void Awake()
    {
        webDecrease = 1;
        currentWebTime = timeToEraseWeb;
        websInPlayer = 0;
    }

    void Update()
    {
        EraseWeb();
    }

    private void EraseWeb()
    {
        if (websInPlayer == 0)
            return;

        currentWebTime -= Time.deltaTime;

        if(currentWebTime <= 0)
        {
            websInPlayer--;
            PlayerSpeedWithWebs();

            if (websInPlayer == 0)
                return;

            currentWebTime = timeToEraseWeb;
        }
    }

    public float PlayerSpeedWithWebs()
    {
        switch (websInPlayer) 
        {
            case 0:
                webDecrease = 1;
                break;
            case 1:
                webDecrease = speedDecreaseWithOneWeb;
                break;
            case 2:
                webDecrease = speedDecreaseWithTwoWebs;
                break;
            case 3:
                webDecrease = speedDecreaseWithThreeWebs;
                break;
            default: 
                break;
        }
        return webDecrease;
    }

    public int GetWebs()
    {
        return websInPlayer;
    }

    public void EraseAllWebs()
    {
        websInPlayer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (websInPlayer == 3)
            return;

        if(collision.CompareTag("Web"))
        {
            websInPlayer++;
            PlayerSpeedWithWebs();
            currentWebTime = timeToEraseWeb;
        }
    }
}
