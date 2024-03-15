using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TpButton : MonoBehaviour
{
    public TpObject tpObject;

    TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    public MenuNavegation menuNavegation;

    public void Initialize()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = tpObject.id.ToString();

        Button bt = GetComponent<Button>();
        bt.onClick.AddListener(() => SetTp());
    }

    public void SetTp()
    {
        SelectTpsManager.instance.SetIdToTeleport(tpObject.id);
        menuNavegation.GoToGame();
    }
}
