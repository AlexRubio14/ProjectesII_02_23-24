using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LongClickButtonController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference pressedAction; 

    private bool pointDown;
    private float pointerDownTimer;

    [SerializeField]
    private float requiredHoldTime;

    public UnityEvent onLongClick;

    [SerializeField]
    private Image fillImage;

    private void OnEnable()
    {
        pressedAction.action.started += ButtonPressed;
        pressedAction.action.canceled += ButtonPressed;
    }
    private void OnDisable()
    {
        pressedAction.action.started -= ButtonPressed;
        pressedAction.action.canceled -= ButtonPressed;
    }

    private void ButtonPressed(InputAction.CallbackContext obj)
    {
        pointDown = obj.started; 
    }

    public void Update()
    {
        if (pointDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer > requiredHoldTime)
            {
                if (onLongClick != null)
                    onLongClick.Invoke();

                Reset();
            }
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
        }
        else
        {
            if (pointerDownTimer > 0)
            {
                pointerDownTimer -= Time.deltaTime;
                fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
            }
        }
    }

    private void Reset()
    {
        pointDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }
}
