using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField]
    AudioClip gameMusic;

    private string mixerGroup;

    private AudioSource _as;

    private void Start()
    {
        mixerGroup = "Music";

        _as = AudioManager.instance.Play2dLoop(gameMusic, mixerGroup, 1, 1, 1);
    }

    private void OnDisable()
    {
        AudioManager.instance.StopLoopSound(_as);
    }
}
