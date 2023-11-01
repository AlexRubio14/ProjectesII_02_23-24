using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CannonController : MonoBehaviour
{
    [SerializeField]
    public List<Transform> l_lasers;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private float reloadDelay;

    private bool canShoot;
    private float currentDelay;

    [SerializeField]
    private Camera camera;

    public UnityEvent onShoot = new UnityEvent();
    public UnityEvent<Vector2> onMoveTurret = new UnityEvent<Vector2>();

    [SerializeField]
    private Transform turretParent;

    [SerializeField]
    private float rotationSpeed;

    private void Awake()
    {
     
        canShoot = true;

        if(camera == null)
            camera = Camera.main;

    }

    private void Update()
    {
        if(canShoot == false)
        {
            currentDelay -= Time.deltaTime;
            if(currentDelay <= 0)
            {
                canShoot=true;
            }
        }

        //GetTurretMovement();
        //Movement(GetMousePosition());
        //GetShootInput();
    }
    private void Movement(Vector2 pointerPosition)
    {
        Vector3 turretDirection = (Vector3)pointerPosition - turretParent.position;

        var desiredAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg;

        turretParent.rotation = Quaternion.RotateTowards(turretParent.rotation, Quaternion.Euler(0, 0, desiredAngle - 90), rotationSpeed);
    }

    void GetShootInput()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            onShoot?.Invoke();
        }
    }
    void GetTurretMovement()
    {
        onMoveTurret?.Invoke(GetMousePosition());
    }

    Vector2 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = camera.nearClipPlane;
        Vector2 mouseWoeldPosition = camera.ScreenToWorldPoint(mousePosition);
        return mouseWoeldPosition;
    }

    public void Shoot()
    {
        if(canShoot) 
        {
            canShoot = false;
            currentDelay = reloadDelay;
            foreach(var laser in l_lasers)
            {
                GameObject l = Instantiate(laserPrefab);
                l.transform.position = laser.transform.position;
                l.transform.localRotation = laser.rotation;
                l.GetComponent<Laser>().Initialize();  
            }
        }
    }
}
