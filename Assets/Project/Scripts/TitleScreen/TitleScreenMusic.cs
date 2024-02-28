using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    AudioClip titleScreenAudio;

    private AudioSource audioSource;

    private string mixerGroup;


    private void OnEnable()
    {
        mixerGroup = "Music";

        audioSource = AudioManager._instance.Play2dLoop(titleScreenAudio, mixerGroup, 1,1,1);
    }

    private void OnDisable()
    {
        AudioManager._instance.StopLoopSound(audioSource);
    }
}
