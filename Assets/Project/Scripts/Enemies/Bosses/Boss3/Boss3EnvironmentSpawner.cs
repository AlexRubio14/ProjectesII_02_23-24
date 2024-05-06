using UnityEngine;

public class Boss3EnvironmentSpawner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<Boss3Controller>().PlaceNewEnvironment();
        }
    }
}
