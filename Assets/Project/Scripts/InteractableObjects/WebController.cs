using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem webDestroyedParticles;

    private void Start()
    {
        webDestroyedParticles.gameObject.transform.SetParent(null);

    }


    private void OnDisable()
    {
        if (!webDestroyedParticles)
            return;

        webDestroyedParticles.gameObject.SetActive(true);
    }
}
