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
            CreateTpButton(tp);
        }
    }

    protected void UpdateDiscoveredTpList()
    {
        List<TpObject> tpList = SelectTpsManager.instance.tpList;
        for (int i = 0; i < discoveredTpButtonList.Count; i++)
        {
            Button bt = discoveredTpButtonList[i].GetComponent<Button>();
            bt.interactable = tpList[i].discovered;
        }
    }

    protected void CreateTpButton(TpObject tp)
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


    public abstract void OnButtonClick(int id);

    protected virtual void OnEnable()
    {
        SelectLastTpUsed();
    }    
    
    protected void SelectLastTpUsed()
    {
        List<TpObject> tpList = SelectTpsManager.instance.tpList;
        int currentId = SelectTpsManager.instance.GetIdToTeleport();

        for (int i = 0; i < discoveredTpButtonList.Count; i++)
        {
            if (currentId == tpList[i].id)
            {
                discoveredTpButtonList[i].GetComponent<Button>().Select();
            }
        }
    }
}
