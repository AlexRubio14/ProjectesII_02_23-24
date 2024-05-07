using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss3AvalancheController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().SubstractFuelPercentage(100);
            collision.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 100, ForceMode2D.Impulse);
        }
    }
}
