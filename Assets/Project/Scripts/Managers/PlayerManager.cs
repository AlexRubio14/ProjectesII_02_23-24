using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [field: SerializeField]
    public PlayerController player { get; private set; }

    private void Awake()
    {
        if (player == null)
        {
            enabled = false;
            Destroy(this);
            return;

            
        }else if (Instance != null && Instance != this)
        {
            Instance.enabled = false;
            Destroy(Instance);
        }

        Instance = this;
    }
}
