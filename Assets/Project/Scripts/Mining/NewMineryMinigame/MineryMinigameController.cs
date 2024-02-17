using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MineryMinigameController : MonoBehaviour
{

    [Header("Input"), SerializeField]
    private InputActionReference leftLaserTrigger;
    [SerializeField]
    private InputActionReference rightLaserTrigger;


    [Space, Header("Lasers"), SerializeField]
    private float laserRayGrowSpeed;
    public bool leftLaserOn {  get; private set; }
    public bool rightLaserOn {  get; private set; }
    public float leftLaserRayProgress {  get; private set; }
    public float rightLaserRayProgress { get; private set; }


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }


    private void OnEnable()
    {
        leftLaserTrigger.action.started += LeftLaserAction;
        leftLaserTrigger.action.canceled += LeftLaserAction; 
        rightLaserTrigger.action.started += RightLaserAction;
        rightLaserTrigger.action.canceled += RightLaserAction;
    }
    private void OnDisable()
    {
        leftLaserTrigger.action.started -= LeftLaserAction;
        leftLaserTrigger.action.canceled -= LeftLaserAction;
        rightLaserTrigger.action.started -= RightLaserAction;
        rightLaserTrigger.action.canceled -= RightLaserAction;
    }

    // Update is called once per frame
    private void Update()
    {
        SetLaserRaysProcess();
    }


    #region Laser Rays
    private void SetLaserRaysProcess() 
    {
        leftLaserRayProgress += leftLaserOn ? laserRayGrowSpeed * Time.deltaTime : -laserRayGrowSpeed * Time.deltaTime;
        rightLaserRayProgress += rightLaserOn ? laserRayGrowSpeed * Time.deltaTime : -laserRayGrowSpeed * Time.deltaTime;

        leftLaserRayProgress = Mathf.Clamp01(leftLaserRayProgress);
        rightLaserRayProgress = Mathf.Clamp01(rightLaserRayProgress);
    }
    #endregion

    #region Input
    private void LeftLaserAction(InputAction.CallbackContext obj)
    {
        leftLaserOn = obj.action.IsPressed();
        Debug.Log("ENTRA IZQUIERDA, ESTA PRESIONADO? " + leftLaserOn);
    }
    private void RightLaserAction(InputAction.CallbackContext obj)
    {
        rightLaserOn = obj.action.IsInProgress();
        Debug.Log("ENTRA IZQUIERDA, ESTA PRESIONADO? " + rightLaserOn);

    }
    #endregion
}
