using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMapController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> tpPosition = new List<GameObject>();

    [SerializeField]
    private Image SelectedImage;

    private int currentSelectedTp = 0;

    private void Start()
    {
        InitializeTpImages();
        SetSelectedTeleport(0);
    }

    private void OnEnable()
    {
        InitializeTpImages();
    }
    private void InitializeTpImages()
    {
        for (int i = 0; i < SelectTpsManager.instance.tpList.Count; i++)
        {
            tpPosition[i].SetActive(SelectTpsManager.instance.tpList[i].discovered);
        }
    }

    public void SetSelectedTeleport(int tpId)
    {
        tpPosition[currentSelectedTp].SetActive(true);

        SelectedImage.transform.position = tpPosition[tpId].transform.position;

        currentSelectedTp = tpId;
    }
}
