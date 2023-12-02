using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpgradeController : MonoBehaviour
{
    private int interactableMask;

    private void Start()
    {

        interactableMask = LayerMask.NameToLayer("InteractableObject");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == interactableMask)
        {
            //GetComponent de Interactable
            InteractableObject interactableObject = collision.gameObject.GetComponent<InteractableObject>();
            if (interactableObject.isHide)
                interactableObject.UnHide();

        }
    }
}
