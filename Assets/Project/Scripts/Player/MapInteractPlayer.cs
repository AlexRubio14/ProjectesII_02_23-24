using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInteractPlayer : MonoBehaviour
{
    private InteractableObject nearestObject;


    [Header("Interact Detection"), SerializeField]
    private float checkRadius;
    [SerializeField]
    private LayerMask interactableLayer;
    [Space, Header("Interact Canvas"), SerializeField]
    private GameObject c_itemToShowCanvas;
    [SerializeField]
    private Image c_itemToShowImage;
    [SerializeField]
    private GameObject c_lockedUpgradeImage;

    [SerializeField]
    private Sprite interactKeySprite;


    private void FixedUpdate()
    {
        CheckNearInteractableObject();
        ShowNeededUpgrade();

    }
    private void CheckNearInteractableObject()
    {
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, checkRadius, Vector2.zero, 0.0f, interactableLayer);

        if (hit2D)
        {
            nearestObject = hit2D.collider.GetComponent<InteractableObject>();
        }
        else
        {
            nearestObject = null;
        }
    }

    private void ShowNeededUpgrade()
    {

        if (!nearestObject)
        {
            c_itemToShowCanvas.SetActive(false);
            return;
        }
        else if (nearestObject && !c_itemToShowCanvas.activeInHierarchy)
        {
            c_itemToShowCanvas.SetActive(true);
        }

        if (nearestObject.c_upgradeNeeded && ! UpgradeManager.Instance.UpgradeObtained[nearestObject.c_upgradeNeeded])
        {
            c_itemToShowImage.sprite = nearestObject.c_upgradeNeeded.c_UpgradeSprite;
            c_lockedUpgradeImage.SetActive(true);
            return;
        }

        //Mostrar el sprite de pulsar el boton
        c_itemToShowImage.sprite = interactKeySprite;
        c_lockedUpgradeImage.SetActive(false);

    }

    public void InteractNearObject()
    {
        if (!nearestObject)
            return;

        nearestObject.Interact();
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
