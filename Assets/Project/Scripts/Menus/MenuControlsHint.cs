using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class MenuControlsHint : MonoBehaviour
{
    public static MenuControlsHint Instance;
    public enum ActionType { ACCEPT, GO_BACK, MOVE_MENU, SKIP_DIALOGUE };
    public enum HintsPos { TOP_LEFT,  TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT };

    [SerializeField]
    private RectTransform hintsLayout;

    [Space, SerializedDictionary("Position", "Coordinates")]
    public SerializedDictionary<HintsPos, Vector2> hintsPos;

    [Space, SerializedDictionary("Controls", "Sprites")]
    public SerializedDictionary<ActionType, List<Sprite>> actionsSprite;
    [SerializedDictionary("Controls", "Sprites")]
    public SerializedDictionary<ActionType, string> actionsName;

    [Space, SerializeField]
    private Image[] keysSprite;
    [Space, SerializeField]
    private TextMeshProUGUI[] actionsText;

    [HideInInspector]
    public List<ActionType> currentActions;
    private List<string> currentActionName;
    private HintsPos currentHintPosition;  
    
    [HideInInspector]
    public List<ActionType> lastActions;
    private List<string> lastActionName;
    private HintsPos lastHintPosition;  


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnInputDeviceChanged;
    }
    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnInputDeviceChanged;
    }
    private void OnInputDeviceChanged(InputDevice arg1, InputDeviceChange arg2)
    {
        List<ActionType> currLastActions = lastActions;
        UpdateHintControls(currentActions, currentActionName, currentHintPosition);
        lastActions = currLastActions;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            List<ActionType> _actions = new List<ActionType>();
            _actions.Add(ActionType.ACCEPT);

            UpdateHintControls(_actions);
        }
    }

    public void UpdateHintControls(List<ActionType> _actions, List<string> _actionName = null, HintsPos _pos = HintsPos.BOTTOM_LEFT)
    {
  
        for (int i = 0; i < keysSprite.Length; i++)
        {
            keysSprite[i].gameObject.SetActive(false);
            actionsText[i].gameObject.SetActive(false);
        }

        if (_actions == null) 
            return;
        
        for (int i = 0; i < _actions.Count; i++)
        {
            if (i > 4)
                break;

            keysSprite[i].gameObject.SetActive(true);
            keysSprite[i].sprite = actionsSprite[_actions[i]][(int)InputController.Instance.GetCurrentControllerType()];
            actionsText[i].gameObject.SetActive(true);
            if (_actionName != null && _actionName[i] != "")
                actionsText[i].text = _actionName[i];
            else
                actionsText[i].text = actionsName[_actions[i]];
        }

        HorizontalLayoutGroup layout = hintsLayout.GetComponent<HorizontalLayoutGroup>();

        switch (_pos)
        {
            case HintsPos.TOP_LEFT:
                layout.childAlignment = TextAnchor.MiddleLeft;
                hintsLayout.anchorMax = new Vector2(0, 1);
                hintsLayout.anchorMin = new Vector2(0, 1);
                break;
            case HintsPos.TOP_RIGHT:
                layout.childAlignment = TextAnchor.MiddleRight;
                hintsLayout.anchorMax = new Vector2(1, 1);
                hintsLayout.anchorMin = new Vector2(1, 1);
                break;
            case HintsPos.BOTTOM_LEFT:
                layout.childAlignment = TextAnchor.MiddleLeft;
                hintsLayout.anchorMax = new Vector2(0,0);
                hintsLayout.anchorMin = new Vector2(0,0);
                break;
            case HintsPos.BOTTOM_RIGHT:
                layout.childAlignment = TextAnchor.MiddleRight;
                hintsLayout.anchorMax = new Vector2(1, 0);
                hintsLayout.anchorMin = new Vector2(1, 0);
                break;
            default:
                break;
        }
        hintsLayout.anchoredPosition = hintsPos[_pos];

        lastActionName = currentActionName;
        lastHintPosition = currentHintPosition;
        lastActions = currentActions;

        currentActions = _actions;
        currentActionName = _actionName;
        currentHintPosition = _pos;
    }
}
