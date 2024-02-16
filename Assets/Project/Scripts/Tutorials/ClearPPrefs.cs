using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPPrefs : MonoBehaviour
{
    [SerializeField]
    private QuestObject firstQuest;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
            firstQuest.obtainedQuest = false;
            firstQuest.completedQuest = false;
        }
    }
}
