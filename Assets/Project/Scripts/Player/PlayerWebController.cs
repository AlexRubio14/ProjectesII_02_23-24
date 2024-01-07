using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWebController : MonoBehaviour
{
    

    private int websInPlayer;

    [SerializeField]
    private int maxWebs;

    [Space, Header("Player Modifications"), SerializeField]
    private float timeToEraseWeb;
    private float currentWebTime;
    [SerializeField]
    private float webSlow;

    [Space, Header("Particles"), SerializeField]
    private ParticleSystem webDragParticles;
    [SerializeField]
    private int particlePerWeb;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>(); 
    }

    private void Start()
    {
        currentWebTime = timeToEraseWeb;
        websInPlayer = 0;

        UpdateParticleWebDrag();
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
            UpdateParticleWebDrag();
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

        UpdateParticleWebDrag();
    }

    private void UpdateParticleWebDrag()
    {
        if (webDragParticles.emission.rateOverTime.constant == 0 && websInPlayer == 0)
            return;

        ParticleSystem.EmissionModule emission = webDragParticles.emission;
        emission.rateOverTime = websInPlayer * particlePerWeb;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.CompareTag("Web") && websInPlayer <= maxWebs)
        {
            websInPlayer++;
            ReducePlayerSpeed();
            UpdateParticleWebDrag();
            currentWebTime = timeToEraseWeb;
            collision.gameObject.SetActive(false);
        }
    }
}
