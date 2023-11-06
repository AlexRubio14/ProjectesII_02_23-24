using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;


public class CoreController : InteractableObject
{
    private bool glowUp = false;
    [SerializeField]
    private float glowUpSpeed;
    [SerializeField]
    private float timeToChangeScene;

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
    override public void Interact()
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
