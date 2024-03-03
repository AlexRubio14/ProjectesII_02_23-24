using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField]
    private Camera currentCamera;
    [Space, SerializeField]
    private float targetCameraSize;
    private float defaultCameraSize;
    [SerializeField, Range(0.2f, 5f)]
    private float zoomSpeed;
    private float zoomProcess;

    private bool zooming = false;

    private void Start()
    {
        defaultCameraSize = currentCamera.orthographicSize;
    }

    private void FixedUpdate()
    {
        CheckIfZoom();
    }

    private void CheckIfZoom()
    {
        if (!zooming && currentCamera.orthographicSize >= defaultCameraSize)
        {
            zoomProcess -= zoomSpeed * Time.fixedDeltaTime;
            //Zoom In
            Zoom();
        }
        else if(zooming && currentCamera.orthographicSize <= targetCameraSize)
        {
            zoomProcess += zoomSpeed * Time.fixedDeltaTime;
            //Zoom Out
            Zoom();
        }

    }

    private void Zoom()
    {
        zoomProcess = Mathf.Clamp01(zoomProcess);
        currentCamera.orthographicSize = Mathf.Lerp(defaultCameraSize, targetCameraSize, zoomProcess);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            zooming = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            zooming = false;
        }
    }

}
