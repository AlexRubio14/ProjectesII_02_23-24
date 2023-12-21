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
    [SerializeField]
    protected ParticleSystem interactableParticles;
    [SerializeField]
    protected Color vfxUnhideColor;
    protected Color vfxHideColor = new Color(120, 0, 121, 0.1f);
    [SerializeField]
    protected Vector2 particlesSize;
    [SerializeField]
    protected Vector2 particlesPosition;
    [SerializeField]
    protected float hiddenParticlesRateOverTime;

    abstract public void Interact();
    abstract public void UnHide();

    protected void SetupParticles(Color _color)
    {
        //Cambiar el color de las particulas
        ParticleSystem.MainModule color = interactableParticles.main;
        color.startColor = _color;
        ParticleSystem.ShapeModule shape = interactableParticles.shape;
        shape.scale = particlesSize;
        interactableParticles.transform.localPosition = particlesPosition;
        ParticleSystem.EmissionModule emission = interactableParticles.emission;
        emission.rateOverTime = hiddenParticlesRateOverTime;
    }
}
