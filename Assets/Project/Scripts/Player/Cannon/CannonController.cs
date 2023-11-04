using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CannonController : MonoBehaviour
{
    [SerializeField]
    public List<Transform> l_lasers;

    [SerializeField]
    private Transform posToSpawnBullets;

    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private float reloadDelay;
    private float currentDelay;

    private InputController iController;

    private void Awake()
    {
        iController = GetComponentInParent<InputController>();
    }
    private void Update()
    {
        Movement(GetMousePosition());

        currentDelay += Time.deltaTime;


        if (Input.GetButton("Fire1"))
        {
           
            Shoot();        
        }
    }


    private void Movement(Vector2 pointerPosition)
    {
        //Vector2 direction = (pointerPosition - (Vector2)transform.position).normalized;

        //transform.up = direction;
        transform.up = iController.inputAimTurretDirection.normalized;


    }

    Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return mousePosition;
    }

    void Shoot()
    {
        if (currentDelay >= reloadDelay)
        {
            currentDelay = 0;
            Instantiate(laserPrefab, posToSpawnBullets.position, transform.rotation);
            CameraController.Instance.AddLowTrauma();
        }
    }
}
