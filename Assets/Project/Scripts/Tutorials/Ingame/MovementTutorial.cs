using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovementTutorial : Tutorial
{

    [Space, SerializeField]
    private Image rotateImage;
    [SerializeField]
    private Image accelerateImage;
    [SerializeField]
    private Sprite[] rotateSprites;
    [SerializeField]
    private Sprite[] accelerateSprites;

    [Space, SerializeField]
    private float maxDistanceFromPlayer;
    private Vector2 playerStartPos;

    protected override void TutorialMethod()
    {
        
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += UpdateInputImages;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateInputImages;
    }
    public void StartTutorial()
    {
        playerStartPos = PlayerManager.Instance.player.transform.position;
        UpdateInputImages(null, InputDeviceChange.Added);

    }

    public void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        InputController.ControllerType controllerType = InputController.Instance.GetControllerType();
        rotateImage.sprite = rotateSprites[(int)controllerType];

        accelerateImage.gameObject.SetActive(controllerType == InputController.ControllerType.GAMEPAD);
        accelerateImage.sprite = accelerateSprites[(int)controllerType];

    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(playerStartPos, PlayerManager.Instance.player.transform.position) >= maxDistanceFromPlayer)
            EndTutorial();
        
    }

    protected override void EndTutorial()
    {
        gameObject.SetActive(false);
    }

}
