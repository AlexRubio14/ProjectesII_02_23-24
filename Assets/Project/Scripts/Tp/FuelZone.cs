using UnityEngine;

public class FuelZone : MonoBehaviour
{
    [SerializeField]
    private float fuelIncrement;

    [SerializeField]
    private AudioClip healingClip;
    private AudioSource healingSource;

    private FuelZoneSaveItemsController saveItems;
    private void Awake()
    {
        saveItems = GetComponent<FuelZoneSaveItemsController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.fuelConsume += fuelIncrement;

            if (PlayerManager.Instance.player.GetFuel() < PlayerManager.Instance.player.GetMaxFuel() - 3f)
            {
                PlayerManager.Instance.player.refillFuelParticles.Play();
                healingSource = AudioManager.instance.Play2dLoop(healingClip, "HealingSFX");
            }
            saveItems.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerManager.Instance.player.fuel >= PlayerManager.Instance.player.GetMaxFuel()- 1f)
            {
                PlayerManager.Instance.player.refillFuelParticles.Stop();
                if (healingSource)
                {
                    AudioManager.instance.StopLoopSound(healingSource);
                    healingSource = null;
                }
            }
            else if (PlayerManager.Instance.player.refillFuelParticles.isStopped)
            {
                PlayerManager.Instance.player.refillFuelParticles.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.fuelConsume -= fuelIncrement;
            PlayerManager.Instance.player.refillFuelParticles.Stop();

            if (healingSource)
            {
                AudioManager.instance.StopLoopSound(healingSource);
                healingSource = null;
            }
            saveItems.enabled = false;
        }
    }
}
