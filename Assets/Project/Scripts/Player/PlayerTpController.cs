using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTpController : MonoBehaviour
{
    public Action onTpStop;
    public Action onTpStart;
    public Action onTpParticlesStop;

    [SerializeField]
    MonoBehaviour[] playerScripts;

    [SerializeField]
    private SpriteRenderer playerSprite;

    [Space, Header("TP"), SerializeField]
    private GameObject tpParticlesPrefab;

    [SerializeField]
    private float timeToStopParticles;

    private PlayerController playerController;
    private PlayerMapInteraction playerMapInteraction;

    private ParticleSystem tpParticles;

    private Canvas[] activeCanvas;

    [SerializeField]
    private AudioClip teleportClip;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMapInteraction = GetComponent<PlayerMapInteraction>();
    }

    private void Start()
    {
        activeCanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
    }

    public void StartTravel()
    {
        AudioManager.instance.Play2dOneShotSound(teleportClip, "TpInteraction");

        playerController.ChangeState(PlayerController.State.FREEZE);

        EnablePlayer(false);

        DisplayCanvas(false);

        tpParticles = Instantiate(tpParticlesPrefab, playerController.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        
        Invoke("StopParticles", timeToStopParticles);

        TransitionCanvasManager.instance.FadeIn();

        if (onTpStart != null)
            onTpStart();
    }

    public void EnablePlayer(bool isEnabled)
    {
        foreach (MonoBehaviour item in playerScripts)
        {
            item.enabled = isEnabled;
        }

        playerSprite.enabled = isEnabled;
    }

    public void DisplayCanvas(bool IsEnabled)
    {
        playerMapInteraction.showCanvas = IsEnabled;

        foreach (Canvas item in activeCanvas)
        {
            if(item.gameObject != transform.parent.gameObject && item.gameObject != TransitionCanvasManager.instance.gameObject)
                item.gameObject.SetActive(IsEnabled);
        }
    }

    private void StopParticles()
    {
        tpParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        TpEnd();
        //Invoke("TpEnd", timeToEndTp);
        playerController.refillFuelParticles.Stop();


        if (onTpParticlesStop != null)
            onTpParticlesStop();
    }

    private void TpEnd()
    {
        if (onTpStop != null)
            onTpStop();
    }
}
