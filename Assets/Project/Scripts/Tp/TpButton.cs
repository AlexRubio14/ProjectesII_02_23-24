using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TpButton : MonoBehaviour
{
    [HideInInspector]
    public TpObject tpObject;

    TextMeshProUGUI textMeshProUGUI;

    [HideInInspector]
    public MenuNavegation menuNavegation;

    [HideInInspector]
    public GameObject zoneImage;

    public void Initialize()
    {
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
        SelectTpsManager.instance.SetIdToTeleport(tpObject.id);
        ShopMusic.instance.StopMusic();
        menuNavegation.GoToGame();
    }
}
