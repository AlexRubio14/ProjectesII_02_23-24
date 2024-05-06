using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossDoors : InteractableObject
{

    [Space, SerializeField]
    private Transform posToSpawnPlayer;
    [SerializeField]
    public Transform posToBack;

    [SerializeField]
    private BossDialogue currentDialogue;

    [SerializeField]
    private Sprite brokenDoorSprite;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        DestroyDoor();

        BossManager.Instance.onBossExit += DestroyDoor;
    }

    private void OnDestroy()
    {
        BossManager.Instance.onBossExit -= DestroyDoor;
    }

    public override void Interact()
    {
        TransitionCanvasManager.instance.FadeIn();
        SubscribeToFadeIn();
        currentDialogue.door = this;
    }

    private void TpPlayer()
    {
        TransitionCanvasManager.instance.onFadeIn -= TpPlayer;
        PlayerManager.Instance.player.transform.position = posToSpawnPlayer.position;
        BossManager.Instance.onBossEnter();
        TransitionCanvasManager.instance.FadeOut();
    }
    public void SubscribeToFadeIn()
    {
        TransitionCanvasManager.instance.onFadeIn += TpPlayer;
    }

    public void DestroyDoor()
    {
        if (!PlayerPrefs.HasKey(currentDialogue.tutorialkey))
            return;

        spriteRenderer.sprite = brokenDoorSprite;
        gameObject.layer = 0;
        GetComponentInChildren<Light2D>().enabled = false;
        enabled = false;
    }

}
