using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TpButton : MonoBehaviour
{
    private TpObject tpObject;

    TextMeshProUGUI textMeshProUGUI;

    private MenuNavegation menuNavegation;

    private MenuMapController menuMapController;

    private DisplayTps displayTps;


    public void Initialize(MenuMapController currentMap, DisplayTps displayTps, MenuNavegation menuNavegation, TpObject tpObject)
    {
        menuMapController = currentMap;
        this.displayTps = displayTps;
        this.menuNavegation = menuNavegation;
        this.tpObject = tpObject;

        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = tpObject.zoneName;

        Button bt = GetComponent<Button>();
        bt.onClick.AddListener(() => SetTp());
    }

    public void SelectButton()
    {
        Button bt = GetComponent<Button>();
        bt.Select();
    }

    public void SetTp()
    {
        displayTps.OnButtonClick(tpObject.id);
    }

    public TpObject GetTp()
    {
        return tpObject;
    }

    public void BackToHub()
    {
        menuNavegation.GoToHub();
    }

    public void OnSelectButton()
    {
        menuMapController.SetSelectedTeleport(tpObject.id - 1);
    }
}
