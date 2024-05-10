using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuBackController : MonoBehaviour
{
    public static MenuBackController instance;

    [SerializeField]
    private InputActionReference backMenuAction;

    public Button backButton;

    public bool canGoBack = true;

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
    private void OnDisable()
    {
        backMenuAction.action.canceled -= BackAction;

        instance = null;
    }
    private void BackAction(InputAction.CallbackContext obj)
    {
        if (canGoBack && backButton && backButton.interactable && backButton.gameObject.activeInHierarchy)
        {
            Button currentButton = backButton;
            backButton = null;
            currentButton.onClick.Invoke();
        }
    }
}
