using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [field: SerializeField]
    public GameObject inventory { get; private set; }

    public void ChangeInventoryVisibility()
    {
        inventory.SetActive(!inventory.activeInHierarchy);
    }
}
