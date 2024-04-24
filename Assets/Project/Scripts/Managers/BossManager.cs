using UnityEngine;
using System;
public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    public Action onBossEnter;
    public Action onBossExit;

    public bool onBoss { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;


        onBossEnter += OnBossEnter;
        onBossExit += OnBossExit;
    }


    private void OnDisable()
    {
        onBossEnter -= OnBossEnter;
        onBossExit -= OnBossExit;
    }

    private void OnBossEnter()
    {
        onBoss = true;
    }

    private void OnBossExit()
    {
        onBoss = false;
    }



}
