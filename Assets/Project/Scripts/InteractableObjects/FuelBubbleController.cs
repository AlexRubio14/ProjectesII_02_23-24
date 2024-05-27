using UnityEngine;

public class FuelBubbleController : FloatingItem
{
    [Space, Header("Bubble"), SerializeField]
    private float fuelRecover;

    private void Start()
    {
        animator.SetBool("IsAnimated", true);
    }

    private void OnEnable()
    {
        canChase = false;
        animator.SetTrigger("SpawnAnim");
    }

    protected override void ChaseAction()
    {
        Vector2 dir = (PlayerManager.Instance.player.transform.position - transform.position).normalized;
        rb2d.AddForce(dir * moveSpeed, ForceMode2D.Force);

        CheckGetDistance(PlayerManager.Instance.player.transform);
    }

    protected override void ObtainAction()
    {
        //Sumar combustible
        PlayerManager.Instance.player.SubstractFuelPercentage(-fuelRecover);
        //Spawnear Particulas que sean hijas del player
        PlayerManager.Instance.player.bubbleFuelParticles.Play();


        AudioManager.instance.Play2dOneShotSound(collectClip, "Items");
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            canChase = true;
    }
}
