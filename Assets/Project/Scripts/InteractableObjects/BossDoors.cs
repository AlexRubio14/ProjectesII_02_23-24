using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossDoors : InteractableObject
{

    [Space, SerializeField]
    private Transform posToSpawnPlayer;
    [SerializeField]
    private BossDialogue currentDialogue;

    [SerializeField]
    private Sprite brokenDoorSprite;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (PlayerPrefs.HasKey(currentDialogue.tutorialkey))
            DestroyDoor();

    }

    public override void Interact()
    {
        TransitionCanvasManager.instance.FadeIn();
        SubscribeToFadeIn();
        currentDialogue.door = this;

    }

    public void SubscribeToFadeIn()
    {
        TransitionCanvasManager.instance.onFadeIn += () => PlayerManager.Instance.player.transform.position = posToSpawnPlayer.position;
        TransitionCanvasManager.instance.onFadeIn += TransitionCanvasManager.instance.FadeOut;
    }

    public void DestroyDoor()
    {
        spriteRenderer.sprite = brokenDoorSprite;
        gameObject.layer = 0;
        GetComponentInChildren<Light2D>().enabled = false;
        enabled = false;
    }

}
