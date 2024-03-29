using UnityEngine;
using UnityEngine.UI;

public class DisplayTpsInMenu : DisplayTps
{

    [SerializeField]
    private Button spaceShipButton;

    private void Start()
    {
        CreateDiscoveredTpListInMenu();
    }

    protected void CreateDiscoveredTpListInMenu()
    {
        if (!SelectTpsManager.instance.tpList[0].discovered)
        {
            SelectTpsManager.instance.SetIdToTeleport(1);
            ShopMusic.instance.StopMusic();
            menuNavegation.GoToGame();
        }
        else
        {
            CreateDiscoveredTpList();
        }
    }

    public override void OnButtonClick(int id)
    {
        SelectTpsManager.instance.SetIdToTeleport(id);
        ShopMusic.instance.StopMusic();
        menuNavegation.GoToGame();
    }

    private void OnDisable()
    {
        spaceShipButton.Select();
    }
}
