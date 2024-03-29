using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

    [Header("Interact Hint"), SerializeField]
    private SpriteRenderer interactHint;
    [SerializeField]
    private UpgradeObject lightUpgradeObject;
    [SerializeField]
    private Sprite missingUpgradeSprite;

    [SerializeField]
    private Sprite[] interactKeySprite;

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

        if (!nearestObject || !showCanvas)
        {
            interactHint.gameObject.SetActive(false);
            return;
        }
        else if (nearestObject)
        {
            interactHint.gameObject.SetActive(true);
            interactHint.transform.position = nearestObject.GetNearestTransform().position;
        }


        if (nearestObject.c_upgradeNeeded && !UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded) ||
            nearestObject.isHide && !UpgradeManager.Instance.CheckObtainedUpgrade(lightUpgradeObject))
        {
            interactHint.sprite = missingUpgradeSprite;
        }
        else if (nearestObject.isHide)
        {
            interactHint.sprite = lightUpgradeObject.UpgradeSprite;
        }
        else if (nearestObject.c_upgradeNeeded && UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded) && !nearestObject.isInteractable)
        {
            interactHint.sprite = nearestObject.c_upgradeNeeded.UpgradeSprite;
        }
        else if(nearestObject.isInteractable && 
            (nearestObject.c_upgradeNeeded && UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded) && nearestObject.isInteractable || !nearestObject.c_upgradeNeeded))
        {

            interactHint.sprite = interactKeySprite[(int)InputController.Instance.GetControllerType()];
        }
        else if(nearestObject.c_upgradeNeeded && UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded))
        {
            interactHint.sprite = nearestObject.c_upgradeNeeded.UpgradeSprite;
        }
        else
        {
            interactHint.gameObject.SetActive(false);
        }
    }

    public void InteractNearObject()
    {
        if (nearestObject && //Si existe un objeto cercano
            nearestObject.isInteractable && //Si se puede interactuar con el
            !nearestObject.isHide && //Si no esta oculto
            showCanvas
            )
        {
            //Si no necesita mejora o necesita una mejora y la tiene
            if (!nearestObject.c_upgradeNeeded || 
                nearestObject.c_upgradeNeeded && UpgradeManager.Instance.CheckObtainedUpgrade(nearestObject.c_upgradeNeeded))
            {
                nearestObject.Interact(); //Interactua con el objeto
            }
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
