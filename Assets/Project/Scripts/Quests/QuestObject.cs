using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestObject : ScriptableObject
{
    [field: SerializeField]
    public string questID { private set; get; }

    [field: SerializeField]
    public string questName { private set; get; }

    [field: TextArea, SerializeField]
    public string[] questDialogue { private set; get; }

    [field: TextArea, SerializeField]
    public string questResume { private set; get; }

    [SerializedDictionary("Item", "Amount")]
    public SerializedDictionary<ItemObject, short> neededItems;

    public enum RewardType { UPGRADE, NEW_QUEST };

    //Rewards
    [SerializedDictionary("REWARD", "TYPE")]
    public SerializedDictionary<ScriptableObject, RewardType> rewards;


    public bool obtainedQuest = false;

    public bool completedQuest = false;
}
