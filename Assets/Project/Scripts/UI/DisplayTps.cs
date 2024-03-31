using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DisplayTps : MonoBehaviour
{
    [SerializeField]
    protected Transform buttonsLayout;

    protected List<TpButton> discoveredTpButtonList;

    [SerializeField]
    protected GameObject bt;

    [SerializeField]
    protected MenuNavegation menuNavegation;

    [SerializeField]
    protected GameObject tpMenu;

    [SerializeField]
    protected MenuMapController menuMapController;

    protected void Awake()
    {
        discoveredTpButtonList = new List<TpButton>();
    }

    protected void CreateDiscoveredTpList()
    {
        tpMenu.gameObject.SetActive(true);

        foreach (TpObject tp in SelectTpsManager.instance.tpList)
        {
            GameObject bt = Instantiate(this.bt, buttonsLayout);
            TpButton tpButton = bt.GetComponent<TpButton>();
            discoveredTpButtonList.Add(tpButton);
            tpButton.transform.SetParent(buttonsLayout);
            tpButton.Initialize(menuMapController, this, menuNavegation, tp);

            Button button = bt.GetComponent<Button>();
            if (!tp.discovered)
            {
                button.interactable = false;
            }

            if (tp.id == 1)
                button.Select();
        }
    }

    public abstract void OnButtonClick(int id);

    protected void OnEnable()
    {
        if (discoveredTpButtonList.Count > 0)
            discoveredTpButtonList[0].SelectButton();
    }     
}
