using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public float timeParameter;

    public Action pauseAction;

     public Action resumeAction;

    public static TimeManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        
        DontDestroyOnLoad(this);

        timeParameter = 1.0f;
    }

    public void PauseGame()
    {
        timeParameter = 0.0f;
        pauseAction();
    }

    public void ResumeGame()
    {
        timeParameter = 1.0f;

        if(resumeAction != null)
        {
            resumeAction();
        }
    }
}
