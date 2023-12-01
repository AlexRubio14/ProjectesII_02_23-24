using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electro : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject light = collision.transform.GetChild(4).gameObject; 
            light.SetActive(!(light.activeSelf));
        }
    }
}
