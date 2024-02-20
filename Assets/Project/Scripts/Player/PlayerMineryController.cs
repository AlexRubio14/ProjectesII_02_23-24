using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineryController : MonoBehaviour
{
    [SerializeField]
    private MineMinigameController c_miningMinigame;
    [SerializeField]
    private MineryMinigameController miningMinigame;

    private PlayerController c_playerController;

    private void Awake()
    {
        c_playerController = GetComponent<PlayerController>();
    }

    public void StartMinery(MineralController _mineral)
    {
        c_playerController.ChangeState(PlayerController.State.MINING);
        c_miningMinigame.SetMiningObject(_mineral);
        c_miningMinigame.gameObject.SetActive(true);

        List<MenuControlsHint.ActionType> neededControls = new List<MenuControlsHint.ActionType>();
        neededControls.Add(MenuControlsHint.ActionType.GO_BACK);

        List<string> nameList = new List<string>();
        nameList.Add("Hold to Quit");
        MenuControlsHint.Instance.UpdateHintControls(neededControls, nameList, MenuControlsHint.HintsPos.BOTTOM_LEFT);
    }

    public void StartNewMinery(MineralController _mineral)
    {
        c_playerController.ChangeState(PlayerController.State.MINING);
        miningMinigame.StartMining(_mineral);
    }
}
