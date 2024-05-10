using UnityEngine;

public class PlayerMineryController : MonoBehaviour
{
    [SerializeField]
    private MineMinigameController miningMinigame;

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

}
