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
    [SerializeField]
    protected Color vfxHideColor;
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

    [SerializeField]
    protected Transform[] controlHintPivot;

    [SerializeField]
    protected bool placeParticles = true;
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
        color.startColor = new Color(_color.r, _color.g, _color.b, 0.1f);

        if (!placeParticles)
            return;
        ParticleSystem.ShapeModule shape = interactableParticles.shape;
        shape.scale = particlesSize;
        interactableParticles.transform.localPosition = particlesPosition;
        ParticleSystem.EmissionModule emission = interactableParticles.emission;
        emission.rateOverTime = hiddenParticlesRateOverTime;
    }

    public Transform GetNearestTransform()
    {
        Transform currentTransform = null;
        float lastDistance = 100.0f;

        foreach(Transform t in controlHintPivot)
        {
            float tempDistance = Vector2.Distance(t.position, PlayerManager.Instance.player.transform.position);

            if (tempDistance < lastDistance)
            {
                currentTransform = t;
                lastDistance = tempDistance;

            }
        }

        return currentTransform;
    }
}
