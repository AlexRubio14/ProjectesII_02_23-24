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
    private ImageFloatEffect floatEffect;
    private Light2D coreLight;

    private Camera cam;

    private void Awake()
    {
        coreLight = GetComponentInChildren<Light2D>();
        floatEffect = GetComponent<ImageFloatEffect>();
    }

    private void FixedUpdate()
    {
        if (glowUp)
        {
            float glowUpIncrement = Time.deltaTime * glowUpSpeed;
            coreLight.intensity += glowUpIncrement;
            cam.orthographicSize += glowUpIncrement / 8;
        }
    }
    public override void Interact()
    {
        //Acabar el game
        InventoryManager.Instance.EndRun(true);
        PlayerManager.Instance.player.GetComponent<PlayerController>().ChangeState(PlayerController.State.FREEZE);
        PlayerManager.Instance.player.GetComponent<PlayerMapInteraction>().showCanvas = false;
        glowUp = true;
        floatEffect.enabled = false;
        Invoke("EndGame", timeToChangeScene);

        cam = CameraController.Instance.GetComponent<Camera>();
    }

    private void EndGame()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
