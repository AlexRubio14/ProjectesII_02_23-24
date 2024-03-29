using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
