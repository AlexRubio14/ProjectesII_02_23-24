using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1LaserController : MonoBehaviour
{
    public enum LaserState { CASTING, FADE_IN, LASERING, FADE_OUT };

    public LaserState currentLaserState;

    [SerializeField]
    private SpriteRenderer laser;
    [SerializeField]
    private BoxCollider2D collision;

    [Space, SerializeField]
    private float maxLaserScale;
    [SerializeField]
    private float maxLaserTrailWidth;

    [Space, Header("Cast Laser"), SerializeField]
    public float castLaserDuration;
    private float castLaserTimeWaited;
    [SerializeField]
    private SpriteRenderer trackerSR;
    [SerializeField]
    private float trackerMaxAlpha;

    [Space, Header("Fading In"), SerializeField]
    public float fadeInDuration;
    private float fadeInTimeWaited;
    [Space, Header("Laser"), SerializeField]
    public float laserDuration;
    private float laserTimeWaited;
    [Space, Header("Fading Out"), SerializeField]
    public float fadeOutDuration;
    private float fadeOutTimeWaited;

    private void Start()
    {
        currentLaserState = LaserState.CASTING;
        castLaserTimeWaited = 0;
        fadeInTimeWaited = 0;
        laserTimeWaited = 0;
        fadeOutTimeWaited = 0;

        collision.enabled = false;

    }


    private void Update()
    {
        switch (currentLaserState)
        {
            case LaserState.CASTING:
                CastLaser();
                break;
            case LaserState.FADE_IN:
                FadeInLaser();
                break;
            case LaserState.LASERING:
                Laser();
                break;
            case LaserState.FADE_OUT:
                FadeOutLaser();
                break;
            default:
                break;
        }
    }

    private void CastLaser() 
    {
        castLaserTimeWaited += Time.deltaTime;

        trackerSR.color = new Color(trackerSR.color.r, trackerSR.color.g, trackerSR.color.b, trackerMaxAlpha * (castLaserTimeWaited / castLaserDuration));

        if (castLaserDuration <= castLaserTimeWaited)
        {
            //Empezar Fade In
            currentLaserState = LaserState.FADE_IN;
        }


    }

    private void FadeInLaser()
    {
        fadeInTimeWaited += Time.deltaTime;
        
        //Hacer desaparecer el tracker
        trackerSR.color = new Color(trackerSR.color.r, trackerSR.color.g, trackerSR.color.b, fadeInDuration - fadeInTimeWaited / fadeInDuration);
        //Hacer Gordo el rayo
        float scaleValue = Mathf.Lerp(0, maxLaserScale, fadeInTimeWaited / fadeInDuration);
        laser.transform.localScale = new Vector3(scaleValue, scaleValue, laser.transform.localScale.z);

        if (fadeInDuration <= fadeInTimeWaited)
        {
            //Activar la colision del laser
            collision.enabled = true;
            currentLaserState = LaserState.LASERING;
            CameraController.Instance.AddHighTrauma();
        }
    }
    
    private void Laser()
    {
        laserTimeWaited += Time.deltaTime;

        if (laserTimeWaited >= laserDuration)
        {
            collision.enabled = false;
            currentLaserState = LaserState.FADE_OUT;
        }
    }

    private void FadeOutLaser() 
    {
        fadeOutTimeWaited += Time.deltaTime;

        //Bajar la escala del rayo
        float scaleValue = Mathf.Lerp(maxLaserScale, 0, fadeOutTimeWaited / fadeOutDuration);
        laser.transform.localScale = new Vector3(scaleValue, scaleValue, laser.transform.localScale.z);


        if (fadeOutTimeWaited >= fadeOutDuration)
        {
            //Acabar aqui (destroy)
            Destroy(gameObject);
        }
    }


    public void SetCast(float _castLaser)
    {
        castLaserDuration = _castLaser;
    }public void SetFadeIn(float _fadeIn)
    {
        fadeInDuration = _fadeIn;
    }
    public void SetLaser(float _laser)
    {
        laserDuration = _laser;
    }
    public void SetFadeOut(float _fadeOut)
    {
        fadeOutDuration = _fadeOut;
    }

}
