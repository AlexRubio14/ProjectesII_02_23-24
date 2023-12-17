using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    private int websInPlayer;

    [SerializeField]
    private float timeToEraseWeb;
    private float currentWebTime;

    [SerializeField]
    private int maxWebs;

    [Space, Header("Speed Decrease"), SerializeField]
    private float webSlow;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        currentWebTime = timeToEraseWeb;
        websInPlayer = 0;
    }

    private void FixedUpdate()
    {
        EraseWeb();
    }
    public void EraseWeb()
    {
        if (websInPlayer == 0)
            return;

        currentWebTime -= Time.fixedDeltaTime;

        if(currentWebTime <= 0)
        {
            websInPlayer--;
            //Bajar la velocidad
            playerController.externalMovementSpeed += webSlow;
            if (websInPlayer == 0)
                return;

            currentWebTime = timeToEraseWeb;
        }
    }

    public void ReducePlayerSpeed()
    {
        Mathf.Clamp(websInPlayer, 0, maxWebs);

        playerController.externalMovementSpeed -= webSlow;
    }

    public void EraseAllWebs()
    {
        for (int i = 0; i < websInPlayer; i++)
        {
            playerController.externalMovementSpeed += webSlow;
        }

        websInPlayer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.CompareTag("Web") && websInPlayer <= maxWebs)
        {
            websInPlayer++;
            ReducePlayerSpeed();
            currentWebTime = timeToEraseWeb;
            collision.gameObject.SetActive(false);
        }
    }
}
