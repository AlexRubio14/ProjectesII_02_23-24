using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuTabsController : MonoBehaviour
{
    [Header("Input Hints"), SerializeField]
    private Image[] inputHint;
    [SerializeField]
    private Sprite[] leftInputHintImage;
    [SerializeField]
    private Sprite[] rightInputHintImage;


    // Start is called before the first frame update
    void OnEnable()
    {
        InputSystem.onDeviceChange += UpdateInputHints;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateInputHints;
    }



    private void UpdateInputHints(InputDevice arg1, InputDeviceChange arg2)
    {
        inputHint[0].sprite = rightInputHintImage[(int)InputController.Instance.GetCurrentControllerType()];
        inputHint[1].sprite = leftInputHintImage[(int)InputController.Instance.GetCurrentControllerType()];
    }
}
