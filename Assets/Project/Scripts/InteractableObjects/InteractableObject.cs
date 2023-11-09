using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class InteractableObject : MonoBehaviour
{

    [field: SerializeField]
    public UpgradeObject c_upgradeNeeded { get; private set; }


    abstract public void Interact();
}
