using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField]
    AudioClip gameMusic;

    private string mixerGroup;

    private void Start()
    {
        mixerGroup = "Music";

        AudioManager._instance.Play2dLoop(gameMusic, mixerGroup, 1, 1, 1);
    }
}
