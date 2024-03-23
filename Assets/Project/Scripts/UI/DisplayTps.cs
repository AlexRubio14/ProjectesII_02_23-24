using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTps : MonoBehaviour
{
    [SerializeField]
    private Transform buttonsLayout;

    private List<TpButton> discoveredTpButtonList;

    [SerializeField]
    private GameObject bt;

    [SerializeField]
    private MenuNavegation menuNavegation;

    [SerializeField]
    private GameObject tpMenu;

    [SerializeField]
    private Button spaceShipButton;

    [SerializeField]
    private MenuMapController menuMapController;

    private void Awake()
    {
        discoveredTpButtonList = new List<TpButton>();
    }

    private void Start()
    {
        CreateDiscoveredTpList();
    }

    private void CreateDiscoveredTpList()
    {
        if (!SelectTpsManager.instance.tpList[0].discovered)
        {
            SelectTpsManager.instance.SetIdToTeleport(1);
            menuNavegation.GoToGame();
        }
        else
        {
            tpMenu.gameObject.SetActive(true);

            foreach (TpObject tp in SelectTpsManager.instance.tpList)
            {
                bt = Instantiate(bt, buttonsLayout);
                TpButton tpButton = bt.GetComponent<TpButton>();
                discoveredTpButtonList.Add(tpButton);
                tpButton.tpObject = tp;
                tpButton.transform.SetParent(buttonsLayout);
                tpButton.Initialize(menuMapController);

                Button button = bt.GetComponent<Button>();
                if (!tp.discovered)
                {
                    button.interactable = false;
                }

                if (tp.id == 1)
                    button.Select();

                tpButton.menuNavegation = menuNavegation;
            }
        }
    }

    private void OnEnable()
    {
        if (discoveredTpButtonList.Count > 0)
            discoveredTpButtonList[0].SelectButton();
    }     

    private void OnDisable()
    {
        spaceShipButton.Select();
    }
}
