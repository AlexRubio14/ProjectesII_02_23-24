using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelZoneSaveItems : MonoBehaviour
{
    [SerializeField]
    private FuelZoneItemController[] saveItemPool;

    [Space, SerializeField]
    private float timeToSpawnItem;
    private float timeToSpawnWaited;

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
            FuelZoneItemController fuelItem = GetUnUsedItem();

            if (fuelItem)
            {
                fuelItem.gameObject.SetActive(true);
                fuelItem.Initialize(PlayerManager.Instance.player.transform.position, transform.parent.position, curretItem.PickableSprite);
            }
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

    private FuelZoneItemController GetUnUsedItem()
    {
        foreach (FuelZoneItemController item in saveItemPool)
        {
            if (!item.gameObject.activeInHierarchy)
                return item;
        }

        return null;
    }

}
