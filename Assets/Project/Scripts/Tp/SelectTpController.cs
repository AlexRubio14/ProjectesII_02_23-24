using UnityEngine;

public class SelectTpController : MonoBehaviour
{
    public TpObject tp;

    public Transform tpPosition;

    private void Start()
    {
        if (SelectTpsManager.instance.GetIdToTeleport() == tp.id)
            PlayerManager.Instance.player.transform.position = tpPosition.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SelectTpsManager.instance.AddDiscoveredTp(tp.id);
            SelectTpsManager.instance.SetIdToTeleport(tp.id);
        }
    }
}
