using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTps : MonoBehaviour
{
    private int numOfTps;

    [SerializeField]
    private GridLayoutGroup buttonsLayout;

    [SerializeField]
    private GameObject bt;
    

    private void Awake()
    {
        numOfTps = SelectTpsManager.instance.GetSizeOfTpList();
        CreateButtonsAndAddToList();
    }

    public void CreateButtonsAndAddToList()
    {
        for (int i = 1; i < numOfTps; i++)
        {
            GameObject newButton = Instantiate(bt);
            newButton.transform.parent = buttonsLayout.transform;
            
            GetComponent<TpButton>().SetId(i);

            TextMeshProUGUI text = newButton.GetComponentInChildren<TextMeshProUGUI>();
            text.text = i.ToString();
        }
    }
    
    public void SetTpToTeleport()
    {
        int id = GetComponent<TpButton>().GetId();
        SelectTpsManager.instance.SetIdToTeleport(id);
    }
}
