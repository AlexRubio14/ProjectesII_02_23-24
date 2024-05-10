using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMapController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> tpPosition = new List<GameObject>();

    [SerializeField]
    private Image selectedImage;

    [SerializeField]
    private Image spaceshipImage;
    [SerializeField]
    private float spaceshipOffset;

    private int currentSelectedTp = 0;

    private void Start()
    {
        InitializeTpImages();
        SetSelectedTeleport(0);
    }

    private void OnEnable()
    {
        InitializeTpImages();
        StartCoroutine(SelectShipPosition());
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

        selectedImage.transform.position = tpPosition[tpId].transform.position;

        currentSelectedTp = tpId;
    }

    private IEnumerator SelectShipPosition()
    {
        yield return new WaitForEndOfFrame();

        if(spaceshipImage != null)
            spaceshipImage.transform.position = 
                tpPosition[SelectTpsManager.instance.GetIdToTeleport() - 1].transform.position + 
                Vector3.up * spaceshipOffset;    
    }
}
