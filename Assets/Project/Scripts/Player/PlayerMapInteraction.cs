using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMapInteraction : MonoBehaviour
{

    [Header("Input"), SerializeField]
    private InputActionReference interactAction;

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

    public bool showCanvas;

    private void Start()
    {
        showCanvas = true;
    }

    private void OnEnable()
    {
        interactAction.action.started += InteractAction;
    }

    private void OnDisable()
    {
        interactAction.action.started -= InteractAction;
    }

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

        if (!nearestObject || !showCanvas || !nearestObject.isInteractable || nearestObject.isHide)
        {
            c_itemToShowCanvas.SetActive(false);
            return;
        }
        else if (nearestObject && !c_itemToShowCanvas.activeInHierarchy)
        {
            c_itemToShowCanvas.SetActive(true);
        }

        if (nearestObject.c_upgradeNeeded && !UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded))
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
        if (nearestObject && //Si existe un objeto cercano
            nearestObject.isInteractable && //Si se puede interactuar con el
            !nearestObject.isHide && //Si no esta oculto
            nearestObject.c_upgradeNeeded && UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded)
            )//Si necesita una mejora y la tiene
        {
            nearestObject.Interact(); //Interactua con el objeto
        }

    }


    #region Input
    private void InteractAction(InputAction.CallbackContext obj)
    {
        InteractNearObject();
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

}
