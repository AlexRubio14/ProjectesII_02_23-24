using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTpController : InteractableObject
{
    [SerializeField]
    private GameObject tpParticles;
    [SerializeField]
    private float timeToGoHub;
    
    private PlayerController c_playerController;
    private PlayerMapInteraction c_playerMapInteraction;
    private SpriteRenderer c_playerSR;

    private ParticleSystem c_tpParticles;
    private void Start()
    {
        c_playerController = PlayerManager.Instance.player.GetComponent<PlayerController>();
        c_playerMapInteraction = c_playerController.GetComponent<PlayerMapInteraction>();
        c_playerSR = c_playerController.GetComponentInChildren<SpriteRenderer>();
    }

    public override void Interact()
    {
        c_playerController.ChangeState(PlayerController.State.FREEZE);
        c_tpParticles = Instantiate(tpParticles, c_playerController.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        c_playerSR.enabled = false;
        c_playerMapInteraction.showCanvas = false;
        Canvas[] activeCanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (Canvas item in activeCanvas)
        {
            item.gameObject.SetActive(false);
        }
        Invoke("StopParticles", timeToGoHub);
    }
    public override void UnHide()
    {
        Debug.LogWarning("No hay ninguna interaccion");
    }
    private void StopParticles()
    {
        InventoryManager.Instance.EndRun(true);
        c_tpParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Invoke("GoToHub", 3);
    }


    private void GoToHub()
    {
        SceneManager.LoadScene("HubScene");
    }
}
