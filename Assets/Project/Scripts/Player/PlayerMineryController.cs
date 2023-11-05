using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineryController : MonoBehaviour
{
    [SerializeField]
    private MineMinigameController c_miningMinigame;

    private PlayerController c_playerController;

    private void Awake()
    {
        c_playerController = GetComponent<PlayerController>();
    }

    public void CheckMineralNear(MineralController _mineral)
    {
        c_playerController.ChangeState(PlayerController.State.MINING);
        c_miningMinigame.SetMiningObject(_mineral);
        c_miningMinigame.gameObject.SetActive(true);

    }
}
