using System.Collections.Generic;
using UnityEngine;

public class FuelZoneSaveItemsController : MonoBehaviour
{
    [SerializeField]
    private FuelZoneItemController[] saveItemPool;
    [SerializeField]
    private ParticleSystem[] particlesPool;

    [Space, SerializeField]
    private float timeToSpawnItem;
    private float timeToSpawnWaited;


    [Space, SerializeField]
    private AudioClip dropItemClip;

    private void OnEnable()
    {
        timeToSpawnWaited = 0;
    }

    private void Update()
    {
        CheckSpawnItem();
    }

    private void CheckSpawnItem()
    {
        timeToSpawnWaited += Time.deltaTime;
        if (timeToSpawnItem <= timeToSpawnWaited)
        {
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        timeToSpawnWaited = 0;

        ItemObject curretItem = GetItem();

        if (curretItem)
        {
            InventoryManager.Instance.SaveRunItem(curretItem, 1);
            FuelZoneItemController fuelItem = GetUnusedItem();

            if (fuelItem)
            {
                fuelItem.gameObject.SetActive(true);
                fuelItem.Initialize(PlayerManager.Instance.player.transform.position, transform.parent.position, curretItem.PickableSprite);
            }

            AudioManager.instance.Play2dOneShotSound(dropItemClip, "TpInteraction", 1.1f, 0.6f, 1.4f);

        }

    }

    private ItemObject GetItem()
    {
        foreach (KeyValuePair<ItemObject, short> item in InventoryManager.Instance.GetRunItems())
        {
            if (item.Value > 0)
                return item.Key;
        }

        return null;
    }

    private FuelZoneItemController GetUnusedItem()
    {
        foreach (FuelZoneItemController item in saveItemPool)
        {
            if (!item.gameObject.activeInHierarchy)
                return item;
        }

        return null;
    }
    public ParticleSystem GetUnusedParticles()
    {
        foreach (ParticleSystem item in particlesPool)
        {
            if (!item.isPlaying)
                return item;
        }

        return null;
    }

}
