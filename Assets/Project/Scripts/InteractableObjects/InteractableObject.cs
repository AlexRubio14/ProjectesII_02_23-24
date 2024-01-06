using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

abstract public class InteractableObject : MonoBehaviour
{

    [field: SerializeField, Tooltip("Solo es necesario asignarle algo si podemos interactuar con el objeto y necesita una mejora")]
    public UpgradeObject c_upgradeNeeded { get; private set; }

    [SerializeField]
    public bool isInteractable = true;    

    [field: Space, Header("Hide"), SerializeField]
    public bool isHide { get; protected set; }

    [SerializeField, Tooltip("Solo hay que darle un valor si el elemento esta oculto")]
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
    [SerializeField]
    private GameObject unhideVFX;
    [SerializeField]
    private Transform unhidePivot;

    abstract public void Interact();
    virtual public void UnHide()
    {
        GameObject vfx = Instantiate(unhideVFX, unhidePivot.position, Quaternion.identity);

        Destroy(vfx, 3);
    }

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
