using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineryController : MonoBehaviour
{
    [SerializeField]
    private float checkRadius;
    [SerializeField]
    private MineMinigameManager c_miningMinigame;
    [SerializeField]
    private LayerMask mineralsMask;

    private PlayerController c_playerController;

    private void Awake()
    {
        c_playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckMineralNear();
        }
    }

    private void CheckMineralNear()
    {
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, checkRadius, Vector2.zero, 0.0f , mineralsMask);

        if (hit2D)
        {
            c_playerController.ChangeState(PlayerController.State.MINING);
            MineralController currentMineral = hit2D.collider.GetComponent<MineralController>();
            c_miningMinigame.SetMiningObject(currentMineral);
            c_miningMinigame.gameObject.SetActive(true);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
