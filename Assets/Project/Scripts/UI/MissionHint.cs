using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionHint : MonoBehaviour
{
    [SerializeField]
    private List<QuestObject> missionsToHint;

    [SerializeField]
    private TextMeshProUGUI hintText;
    [Space, SerializeField]
    private string allMissionsCompleteString;
    private int currentMissionIndex;

    private ImageFloatEffect floatEffect;
    private TextColorLerpEffect textColorEffect;

    private void Awake()
    {
        floatEffect = GetComponentInChildren<ImageFloatEffect>();
        textColorEffect = GetComponentInChildren<TextColorLerpEffect>();
        floatEffect.canFloat = false;
        textColorEffect.canLerp = false;
        currentMissionIndex = 0;
    }

    private void Start()
    {
        for (int i = 0; i < missionsToHint.Count; i ++)
        {
            if (!missionsToHint[i].completedQuest)
            {
                currentMissionIndex = i;
                break;
            }
        }

        SetUpMissionActions();
        CheckCurrentObjective(null, 0);
    }

    private void SetUpMissionActions()
    {
        InventoryManager.Instance.obtainItemAction += CheckCurrentObjective;
    }

    private void CheckCurrentObjective(ItemObject _item, short _amount)
    {
        if (missionsToHint[missionsToHint.Count - 1].completedQuest)
        {
            hintText.text = allMissionsCompleteString;
            floatEffect.canFloat = true;
            textColorEffect.canLerp = true;
            return;
        }

        foreach (KeyValuePair<QuestObject.QuestObjectives, string> item in missionsToHint[currentMissionIndex].missionHintDictionary)
        {
            switch (item.Key)
            {
                case QuestObject.QuestObjectives.NEED_ALL_MATERIALS:
                    if (!CheckAllMaterials())
                    {
                        hintText.text = item.Value;
                        return;
                    }
                    break;
                case QuestObject.QuestObjectives.NEED_ONE_MATERIAL:
                    if (!CheckOneMaterials())
                    {
                        hintText.text = item.Value;
                        return;
                    }
                    break;
                case QuestObject.QuestObjectives.GO_BASE:
                    hintText.text = item.Value;
                    floatEffect.canFloat = true;
                    textColorEffect.canLerp = true;
                    break;
                default:
                    break;
            }
        }
    }

    private bool CheckAllMaterials()
    {
        foreach(KeyValuePair<ItemObject, short> item in missionsToHint[currentMissionIndex].neededItems)
        {
            if (InventoryManager.Instance.GetTotalItemAmount(item.Key) < item.Value)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckOneMaterials()
    {
        if (InventoryManager.Instance.GetTotalItemAmount(missionsToHint[currentMissionIndex].neededMaterial) < missionsToHint[currentMissionIndex].neededItems[missionsToHint[currentMissionIndex].neededMaterial])
        {

            return false;
        }

        return true;
    }

    public void changeHintString(QuestObject.QuestObjectives _missionState)
    {
        hintText.text = missionsToHint[currentMissionIndex].missionHintDictionary[_missionState];
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.obtainItemAction -= CheckCurrentObjective;
    }
}
