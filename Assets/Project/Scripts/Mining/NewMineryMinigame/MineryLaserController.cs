using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineryLaserController : MonoBehaviour
{
    [SerializeField]
    private Transform pointer;
    [SerializeField]
    private Transform[] lasersPivots;
    [SerializeField]
    private LineRenderer[] lasersLineRend;

    [SerializeField]
    private MineryMinigameController mineryController;

    private void OnEnable()
    {
        
    }

    void Update()
    {
        SetLasersPosition();
    }


    private void SetLasersPosition()
    {
        SetLaserPosition(0, mineryController.rightLaserRayProgress);
        SetLaserPosition(1, mineryController.leftLaserRayProgress);
    }

    private void SetLaserPosition(int _laserId, float _laserProgress)
    {
        lasersLineRend[_laserId].SetPosition(0, lasersPivots[_laserId].position);
        Vector3 endLaserPos = Vector3.Lerp(lasersPivots[_laserId].position, pointer.position, _laserProgress);
        lasersLineRend[_laserId].SetPosition(1, endLaserPos);
    }

}
