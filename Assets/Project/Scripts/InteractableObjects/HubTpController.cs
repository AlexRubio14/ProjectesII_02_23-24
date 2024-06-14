using UnityEngine;

public class HubTpController : InteractableObject
{

    [Space, SerializeField]
    private DisplayTpsInMap fastTravelCanvas;

    [SerializeField]
    private GamepadRumbleManager.Rumble enterTPGamepadRumble;

    public override void Interact()
    {
        fastTravelCanvas.gameObject.SetActive(true);
        fastTravelCanvas.OpenMenu();
        GamepadRumbleManager.Instance.AddRumble(enterTPGamepadRumble);
    }
}
