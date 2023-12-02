using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

abstract public class InteractableObject : MonoBehaviour
{

    [SerializeField]
    public bool isInteractable = true;
    [field: SerializeField]
    public bool isHide { get; protected set; }
    [field: SerializeField, Tooltip("Solo es necesario asignarle algo si podemos interactuar con el objeto y necesita una mejora")]
    public UpgradeObject c_upgradeNeeded { get; private set; }

    [Space, SerializeField, Tooltip("Solo hay que darle un valor si el elemento esta oculto")]
    protected Grid grid;
    [SerializeField, Tooltip("Solo hay que darle un valor si el elemento esta oculto")]
    protected Tilemap tilemap;

    abstract public void Interact();
    abstract public void UnHide();
}
