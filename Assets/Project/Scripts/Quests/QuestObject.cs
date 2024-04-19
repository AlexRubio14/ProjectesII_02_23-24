using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestObject : ScriptableObject
{

    [field: SerializeField]
    public int questID { private set; get; }

    [field: SerializeField]
    public string questTitle { private set; get; }

    [field: SerializeField]
    public string questName { private set; get; }

    [field: TextArea, SerializeField]
    public string[] questDialogue { private set; get; }

    [field: TextArea, SerializeField]
    public string[] questDialogueEnd { private set; get; }

    //[field: TextArea, SerializeField]
    //public string questResume { private set; get; }

    [SerializedDictionary("Item", "Amount")]
    public SerializedDictionary<ItemObject, short> neededItems;

    public enum RewardType { UPGRADE, NEW_QUEST, POWER_UP };

    //Rewards
    [SerializedDictionary("REWARD", "TYPE")]
    public SerializedDictionary<ScriptableObject, RewardType> rewards;


    public bool obtainedQuest = false;
    public bool completedQuest = false;
    public bool newQuest = true;

    public enum QuestState { MISSION_STARTED, MAIN_OBJECTIVE_DONE, MISSION_COMPLETED}

    [SerializedDictionary("MISSION STATE", "STRING")]
    public SerializedDictionary<QuestState, string> missionHintDictionary;
}
