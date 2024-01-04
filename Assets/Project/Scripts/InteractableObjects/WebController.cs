using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem webDestroyedParticles;

    private void Start()
    {
        webDestroyedParticles.gameObject.transform.SetParent(transform.parent);

    }
    private void OnDisable()
    {
        webDestroyedParticles.gameObject.SetActive(true);
    }
}
