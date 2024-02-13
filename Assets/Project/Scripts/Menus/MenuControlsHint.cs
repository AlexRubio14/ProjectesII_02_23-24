using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuControlsHint : MonoBehaviour
{
    public static MenuControlsHint Instance;
    public enum ActionType { ACCEPT, GO_BACK, MOVE_MENU, SKIP_DIALOGUE };

    [SerializedDictionary("Controls", "Sprites")]
    public SerializedDictionary<ActionType, List<Sprite>> actionsSprite;
    [SerializedDictionary("Controls", "Sprites")]
    public SerializedDictionary<ActionType, string> actionsName;

    [Space, SerializeField]
    private Image[] keysSprite;
    [Space, SerializeField]
    private TextMeshProUGUI[] actionsText;

    [HideInInspector]
    public List<ActionType> currentActions;
    [HideInInspector]
    public List<ActionType> lastActions;
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

    public void UpdateHintControls(List<ActionType> _actions)
    {
        for (int i = 0; i < keysSprite.Length; i++)
        {
            keysSprite[i].gameObject.SetActive(false);
            actionsText[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _actions.Count; i++) 
        {
            if (i > 4)
                break;

            keysSprite[i].gameObject.SetActive(true);
            keysSprite[i].sprite = actionsSprite[_actions[i]][1];
            actionsText[i].gameObject.SetActive(true);
            actionsText[i].text = actionsName[_actions[i]];
        }

        lastActions = currentActions;

        currentActions = _actions;


    }

}
