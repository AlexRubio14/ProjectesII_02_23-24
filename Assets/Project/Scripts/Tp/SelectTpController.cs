using Unity.VisualScripting;
using UnityEngine;

public class SelectTpController : MonoBehaviour
{
    public int id;

    public Transform tpPosition;

    private void Start()
    {
        if (SelectTpsManager.instance.GetIdToTeleport() == id)
            PlayerManager.Instance.player.transform.position = tpPosition.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SelectTpsManager.instance.AddDiscoveredTp(id);
        }
    }
}
