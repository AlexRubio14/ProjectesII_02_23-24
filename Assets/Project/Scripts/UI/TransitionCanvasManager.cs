using System;
using UnityEngine;

public class TransitionCanvasManager : MonoBehaviour
{
    public Action onFadeIn;

    public Action onFadeOut;

    private Animator animator;

    public static TransitionCanvasManager instance;

    private void Awake()
    {
        if(instance != null && instance != this) 
        {
            Destroy(this);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject) ;

        animator = GetComponentInChildren<Animator>();
    }

    public void FadeIn()
    {
        animator.Play("FadeIn");
    }

    public void FadeOut()
    {
        animator.Play("FadeOut");
    }

    public void OnFadeIn()
    {
        if (onFadeIn != null) onFadeIn();
    }

    public void OnFadeOut()
    {
        if(onFadeOut != null) onFadeOut();
    }
}
