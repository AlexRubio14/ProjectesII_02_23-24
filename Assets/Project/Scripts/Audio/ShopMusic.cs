using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMusic : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip shopMusic;

    public static ShopMusic instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    public void PlayMusic()
    {
        audioSource = AudioManager.instance.Play2dLoop(shopMusic, "Music", 1, 1, 1);
    }

    public void StopMusic()
    {
        AudioManager.instance.StopLoopSound(audioSource);
        TransitionCanvasManager.instance.onFadeIn -= instance.StopMusic;
    }
}
