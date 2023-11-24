using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstDialogueManager : MonoBehaviour
{
    public static FirstDialogueManager Instance;

    public bool firstDialogue = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

    }




}
