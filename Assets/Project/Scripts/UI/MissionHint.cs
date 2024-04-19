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

    public int currentMissionIndex {  get; private set; }

    private void Awake()
    {
        currentMissionIndex = 0;
    }

    private void Start()
    {
        for (int i = 0; i < missionsToHint.Count; i ++)
        {
            if (!missionsToHint[i].completedQuest)
            {
                hintText.text = missionsToHint[i].missionHintDictionary[QuestObject.QuestState.MISSION_STARTED];
                currentMissionIndex = i;
                return;
            }
        }
    }

    public void changeHintString(QuestObject.QuestState _missionState)
    {
        hintText.text = missionsToHint[currentMissionIndex].missionHintDictionary[_missionState];
    }
}
