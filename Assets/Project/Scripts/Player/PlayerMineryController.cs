using UnityEngine;

public class PlayerMineryController : MonoBehaviour
{
    [SerializeField]
    private MineMinigameController miningMinigame;
    [SerializeField]
    private MineryMinigameController newMiningMinigame;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void StartMinery(MineralController _mineral)
    {
        playerController.ChangeState(PlayerController.State.MINING);
        miningMinigame.SetMiningObject(_mineral);
        miningMinigame.gameObject.SetActive(true);
    }

    public void StartNewMinery(MineralController _mineral)
    {
        playerController.ChangeState(PlayerController.State.MINING);
        newMiningMinigame.StartMining(_mineral);
    }
}
