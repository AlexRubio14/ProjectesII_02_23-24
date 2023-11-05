using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreController : InteractableObject
{
    public override void Interact()
    {
        //Acabar el game
        InventoryManager.Instance.EndRun(true);
        throw new System.NotImplementedException();
    }
}
