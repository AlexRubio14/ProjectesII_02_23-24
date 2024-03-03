using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuBackController : MonoBehaviour
{
    public static MenuBackController instance;

    [SerializeField]
    private InputActionReference backMenuAction;

    public Button backButton;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        if (instance != null && instance != this)
        {
            return;
        }

        instance = this;
        backMenuAction.action.canceled += BackAction;
    }

    private void BackAction(InputAction.CallbackContext obj)
    {
        if (backButton && backButton.interactable && backButton.gameObject.activeInHierarchy)
        {
            backButton.onClick.Invoke();
            backButton = null;
        }
    }

    private void OnDisable()
    {
        backMenuAction.action.canceled -= BackAction;

        instance = null;
    }
}
