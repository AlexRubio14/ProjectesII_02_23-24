using System.Collections;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    AudioClip titleScreenAudio;

    private AudioSource audioSource;


    private void OnEnable()
    {

        IEnumerator waitAudioManager()
        {
            yield return new WaitForEndOfFrame();
            audioSource = AudioManager.instance.Play2dLoop(titleScreenAudio, "Music", 1, 1, 1);
        }

        StartCoroutine(waitAudioManager());
    }

    private void OnDisable()
    {
        AudioManager.instance.StopLoopSound(audioSource);
    }

}
