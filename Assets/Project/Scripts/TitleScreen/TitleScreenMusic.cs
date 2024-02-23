using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    AudioClip titleScreen;

    private string mixerGroup;

    private void Start()
    {
        mixerGroup = "Music";

        AudioManager._instance.Play2dLoop(titleScreen, mixerGroup);
    }
}
