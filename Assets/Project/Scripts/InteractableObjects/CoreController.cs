using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;


public class CoreController : InteractableObject
{
    [Space, Header("Core Collector"), SerializeField]
    private float glowUpSpeed;
    [SerializeField]
    private float timeToChangeScene;

    private bool glowUp = false;

    private Light2D coreLight;

    private void Awake()
    {
        coreLight = GetComponentInChildren<Light2D>();
    }

    private void Update()
    {
        if (glowUp)
        {
            coreLight.pointLightInnerRadius += Time.deltaTime * glowUpSpeed;
        }
    }
    public override void Interact()
    {
        //Acabar el game
        InventoryManager.Instance.EndRun(true);
        PlayerManager.Instance.player.GetComponent<PlayerController>().ChangeState(PlayerController.State.FREEZE);
        PlayerManager.Instance.player.GetComponent<PlayerMapInteraction>().showCanvas = false;
        glowUp = true;
        Invoke("EndGame", timeToChangeScene);
    }

    private void EndGame()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
