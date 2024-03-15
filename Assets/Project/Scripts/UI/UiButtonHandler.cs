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
        AudioManager.instance.Play2dOneShotSound(clickClip, "Button");
    }

    public void PlayOnHoverClip()
    {
        AudioManager.instance.Play2dOneShotSound(hoverClip, "Button");
    }

  
}
