using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    [SerializeField]
    AudioClip ambientMusic;

    private string mixerGroup;

    private void Start()
    {
        mixerGroup = "Music";

       AudioManager._instance.Play2dLoop(ambientMusic, mixerGroup);
    }
}
