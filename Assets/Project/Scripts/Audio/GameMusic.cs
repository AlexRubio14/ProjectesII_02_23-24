using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField]
    private AudioClip gameMusic;
    [SerializeField]
    private AudioClip bossMusic;

    private string mixerGroup;

    private AudioSource _as;

    private void Start()
    {
        mixerGroup = "Music";

        _as = AudioManager.instance.Play2dLoop(gameMusic, mixerGroup, 1, 1, 1);
    }

    private void OnEnable()
    {
        BossManager.Instance.onBossEnter += ChangeBossMusic;
        BossManager.Instance.onBossExit += ChangeMapMusic;
    }

    private void OnDisable()
    {
        AudioManager.instance.StopLoopSound(_as);
        BossManager.Instance.onBossEnter -= ChangeBossMusic;
        BossManager.Instance.onBossExit -= ChangeMapMusic;
    }
    private void ChangeMapMusic()
    {
        _as.clip = gameMusic;
        _as.volume = 1f;
        _as.Play();
    }
    private void ChangeBossMusic()
    {
        _as.clip = bossMusic;
        _as.volume = 0.65f;
        _as.Play();
    }

}
