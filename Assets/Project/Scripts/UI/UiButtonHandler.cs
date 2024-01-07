using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiButtonHandler : MonoBehaviour
{
    [SerializeField]
    private AudioClip clickClip;
    [SerializeField]
    private AudioClip hoverClip;

    public void PlayOnClickClip()
    {
        AudioManager._instance.Play2dOneShotSound(clickClip, "Button");
    }

    public void PlayOnHoverClip()
    {
        AudioManager._instance.Play2dOneShotSound(hoverClip, "Button");
    }

  
}
