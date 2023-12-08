using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestObject : ScriptableObject
{
    [field: SerializeField]
    public string questName { private set; get; }

    [field: TextArea, SerializeField]
    public string questIntroduction { private set; get; }

    [SerializedDictionary("Item", "Amount")]
    public SerializedDictionary<ItemObject, short> neededItems;

    [field: SerializeField]
    public QuestObject nextQuest { private set; get; }

}
