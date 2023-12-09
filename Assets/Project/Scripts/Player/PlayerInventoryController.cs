using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField]
    public GameObject inventory;

    [SerializeField]
    private InputActionReference inventoryAction;

    private void OnEnable()
    {
        inventoryAction.action.started += InventoryAction;
    }
    private void OnDisable()
    {
        inventoryAction.action.started -= InventoryAction;
    }

    private void InventoryAction(InputAction.CallbackContext obj)
    {
        inventory.SetActive(!inventory.activeInHierarchy);
        Debug.Log("sexo");
    }
}
