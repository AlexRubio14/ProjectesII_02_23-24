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

    public void SetRightLaserValue(bool _value)
    {
        c_miningMinigame.chargingRightLaser = _value;
    }
    public void SetLeftLaserValue(bool _value)
    {
        c_miningMinigame.chargingLeftLaser = _value;
    }

    public void StartMinery(MineralController _mineral)
    {
        c_playerController.ChangeState(PlayerController.State.MINING);
        c_miningMinigame.SetMiningObject(_mineral);
        c_miningMinigame.gameObject.SetActive(true);

    }
}
