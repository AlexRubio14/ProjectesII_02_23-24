using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [SerializeField]
    private QuestObject currentQuest;

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
    
    public QuestObject GetCurrentQuest()
    {
        return currentQuest;
    }
    public void EndCurrentQuest()
    {
        //currentQuest = currentQuest.nextQuest;
    }

}
