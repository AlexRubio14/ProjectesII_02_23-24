using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMusic : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip shopMusic;

    private void Awake()
    {
        audioSource = AudioManager.instance.Play2dLoop(shopMusic, "Music", 1,1,1);
    }

    public void StopMusic()
    {
        AudioManager.instance.StopLoopSound(audioSource);
    }
}
