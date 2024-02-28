using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuTabsController : MonoBehaviour
{
    [Header("Input Actions"), SerializeField]
    private InputActionReference changeWindowRightAction;
    [SerializeField]
    private InputActionReference changeWindowLeftAction;

    [Space, Header("Input Hints"), SerializeField]
    private Image[] inputHint;
    [SerializeField]
    private Sprite[] leftInputHintImage;
    [SerializeField]
    private Sprite[] rightInputHintImage;

    [Space, Header("Change Tab"), SerializeField]
    private GameObject questTab;
    [SerializeField]
    private GameObject settingsTab;
    private PauseMenuController pauseMenu;
    [SerializeField]
    private Button settingsFirstSelectedButton;

    private void Awake()
    {
        pauseMenu = GetComponentInParent<PauseMenuController>();
    }

    private void Start()
    {
        UpdateInputHints(null, InputDeviceChange.Added);
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += UpdateInputHints;

        changeWindowRightAction.action.started += ChangeTab;
        changeWindowLeftAction.action.started += ChangeTab;


        OpenQuestTab();
    }


    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateInputHints;

        changeWindowRightAction.action.started -= ChangeTab;
        changeWindowLeftAction.action.started -= ChangeTab;
    }

    private void UpdateInputHints(InputDevice arg1, InputDeviceChange arg2)
    {
        inputHint[0].sprite = rightInputHintImage[(int)InputController.Instance.GetCurrentControllerType()];
        inputHint[1].sprite = leftInputHintImage[(int)InputController.Instance.GetCurrentControllerType()];
    }

    private void ChangeTab(InputAction.CallbackContext obj)
    {
        if (questTab.activeInHierarchy)
        {
            OpenSettingsTab();
        }
        else
        {
            OpenQuestTab();
        }
    }

    public void OpenQuestTab()
    {
        questTab.SetActive(true);
        settingsTab.SetActive(false);
        pauseMenu.SelectFirstQuestButon();
    }

    public void OpenSettingsTab()
    {
        questTab.SetActive(false);
        settingsTab.SetActive(true);
        settingsFirstSelectedButton.Select();
    }

}
