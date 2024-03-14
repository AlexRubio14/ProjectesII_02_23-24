using UnityEngine;

public class DiscoverTpController : MonoBehaviour
{
    private int id;

    [SerializeField]
    private Transform tpPosition;

    public void GetIdFromManager()
    {
        id = SelectTpsManager._instance.GetTotalIds();
        SelectTpsManager._instance.AddTotalIds(1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetIdFromManager();
            SelectTpsManager._instance.AddDiscoveredTp(id, tpPosition);
        }
    }
}
