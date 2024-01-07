using UnityEngine;

public class LaserCollisionController : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem laserParticles;

    public void InstantiateParticles()
    {
        laserParticles.Play();
    }

    public void DestroyObject()
    {
        Destroy(transform.parent.gameObject);
    }
}
