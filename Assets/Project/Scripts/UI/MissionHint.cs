using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionHint : MonoBehaviour
{
    [SerializeField]
    private List<QuestObject> missionsToHint;

    [SerializeField]
    private TextMeshProUGUI hintText;

    private void Start()
    {
        foreach (QuestObject obj in missionsToHint)
        {
            if (!obj.completedQuest)
            {
                hintText.text = obj.stringMissionObjective;
                return;
            }
        }
    }
}
