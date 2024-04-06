using UnityEngine;

public class HubTpController : InteractableObject
{

    [SerializeField]
    private DisplayTpsInMap fastTravelCanvas;

    public override void Interact()
    {
        fastTravelCanvas.gameObject.SetActive(true);
        fastTravelCanvas.OpenMenu();
    }
}
