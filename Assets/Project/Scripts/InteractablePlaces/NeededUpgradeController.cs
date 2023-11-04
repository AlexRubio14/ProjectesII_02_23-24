using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeededUpgradeController : MonoBehaviour
{
    [SerializeField]
    private UpgradeObject c_upgradeNeeded;

    private NeededUpgradePlayer c_playerUpgrade;

    private void Start()
    {
        c_playerUpgrade = PlayerManager.Instance.player.GetComponent<NeededUpgradePlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }
}
