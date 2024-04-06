using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuUIHintController : MonoBehaviour
{
    public static MenuUIHintController instance;

    [SerializedDictionary("UI Image", "Input Sprites")]
    public SerializedDictionary<Image, Sprite[]> actionsSprites;
    [SerializeField]
    private GameObject inputsObject;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += UpdateInputImages;
        UpdateInputImages(new InputDevice(), InputDeviceChange.Added);
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= UpdateInputImages;
    }

    private void UpdateInputImages(InputDevice arg1, InputDeviceChange arg2)
    {
        foreach (KeyValuePair<Image, Sprite[]> item in actionsSprites)
        {
            item.Key.sprite = item.Value[(int)InputController.Instance.GetControllerType()];
        }
    }


    public void HideInputs()
    {
        inputsObject.SetActive(false);
    }

    public void ShowInputs()
    {
        inputsObject.SetActive(true);
    }

}
