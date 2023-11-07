using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallsController : InteractableObject
{
    public override void Interact()
    {
        //Romper el muro
        Destroy(gameObject);
    }
}
