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
        foreach (TpObject tp in SelectTpsManager.instance.tpList)
        {
            bt = Instantiate(bt, buttonsLayout);
            TpButton tpButton = bt.GetComponent<TpButton>();
            discoveredTpButtonList.Add(tpButton);
            tpButton.tpObject = tp;
            tpButton.transform.SetParent(buttonsLayout);
            tpButton.Initialize();

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
