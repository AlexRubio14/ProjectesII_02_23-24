using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMapController : MonoBehaviour
{
    [SerializeField]
    private List<Image> tpPosition = new List<Image>();

    [SerializeField]
    private Image SelectedImage;

    private int currentSelectedTp = 0;

    private void Start()
    {
        InitializeTpImages();
        SetSelectedTeleport(0);
    }

    private void InitializeTpImages()
    {
        for (int i = 0; i < SelectTpsManager.instance.tpList.Count; i++)
        {
            tpPosition[i].enabled = SelectTpsManager.instance.tpList[i].discovered;
        }
    }

    public void SetSelectedTeleport(int tpId)
    {
        tpPosition[currentSelectedTp].enabled = true;

        tpPosition[tpId].enabled = false;
        SelectedImage.transform.position = tpPosition[tpId].transform.position;

        currentSelectedTp = tpId;
    }
}
