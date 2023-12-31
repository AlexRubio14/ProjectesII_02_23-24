using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound3dController : MonoBehaviour
{
    [SerializeField]
    private bool isLoop;
    [SerializeField]
    private bool playAtStart;
    [SerializeField]
    public AudioClip clip;
    [SerializeField]
    private float radius;
    [Space]
    [SerializeField]
    private Vector2 minMaxPitch = new Vector2(0.75f, 1.25f);
    [SerializeField]
    private float volume = 0.1f;
    private void Start()
    {
        AudioManager._instance.AddNew3dAS();
        if (playAtStart)
        {
            PlaySound();
        }
    }
    public void PlaySound() 
    {
        if (isLoop)
        {
            AudioManager._instance.Play3dLoop(clip, radius, transform.position, minMaxPitch.x, minMaxPitch.y, volume);
        }
        else
        {
            AudioManager._instance.Play3dOneShotSound(clip, radius, transform.position, minMaxPitch.x, minMaxPitch.y, volume);
        }
    }
    public void PlaySound(AudioClip _clip)
    {
        if (isLoop)
        {
            AudioManager._instance.Play3dLoop(_clip, radius, transform.position, minMaxPitch.x, minMaxPitch.y, volume);
        }
        else
        {
            AudioManager._instance.Play3dOneShotSound(_clip, radius, transform.position, minMaxPitch.x, minMaxPitch.y, volume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius * 5);
    }
}
