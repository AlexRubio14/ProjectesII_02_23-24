using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestObject : ScriptableObject
{
    [field: SerializeField]
    public string questName { private set; get; }

    [field: TextArea, SerializeField]
    public string questIntroduction { private set; get; }

    [AYellowpaper.SerializedCollections.SerializedDictionary("Item", "Amount")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<ItemObject, short> neededItems;

    [field: SerializeField]
    public QuestObject nextQuest { private set; get; }

}
