using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTpController : InteractableObject
{
    [Space, Header("TP"), SerializeField]
    private GameObject tpParticles;
    [SerializeField]
    private float timeToGoHub;

    [SerializeField]
    private float timeToStopParticles;

    private PlayerController c_playerController;
    private PlayerMapInteraction c_playerMapInteraction;
    private SpriteRenderer c_playerSR;

    private ParticleSystem c_tpParticles;

    [SerializeField]
    private AudioClip teleportClip;

    private void Start()
    {
        c_playerController = PlayerManager.Instance.player.GetComponent<PlayerController>();
        c_playerMapInteraction = c_playerController.GetComponent<PlayerMapInteraction>();
        c_playerSR = c_playerController.GetComponentInChildren<SpriteRenderer>();
    }

    public override void Interact()
    {
        AudioManager._instance.Play2dOneShotSound(teleportClip, "Teleport");

        c_playerController.ChangeState(PlayerController.State.FREEZE);
        c_playerController.GetComponentInChildren<CannonController>().gameObject.SetActive(false);
        c_tpParticles = Instantiate(tpParticles, c_playerController.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        c_playerSR.enabled = false;
        c_playerMapInteraction.showCanvas = false;
        Canvas[] activeCanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (Canvas item in activeCanvas)
        {
            item.gameObject.SetActive(false);
        }
        Invoke("StopParticles", timeToStopParticles);
    }
    private void StopParticles()
    {
        InventoryManager.Instance.EndRun(true);
        c_tpParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Invoke("GoToHub", timeToGoHub);
    }


    private void GoToHub()
    {
        SceneManager.LoadScene("HubScene");
    }
}
