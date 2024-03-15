using Unity.VisualScripting;
using UnityEngine;

public class SelectTpController : MonoBehaviour
{
    [SerializeField]
    private int id;

    [SerializeField]
    private Transform tpPosition;

    private void Update()
    {
        if (id == SelectTpsManager.instance.GetIdToTeleport())
        {
            PlayerManager.Instance.player.transform.position = tpPosition.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SelectTpsManager.instance.AddDiscoveredTp(id);
        }
    }
}
