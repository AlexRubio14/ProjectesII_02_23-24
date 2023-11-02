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
        Movement(GetMousePosition());
        //GetShootInput();
    }
    private void Movement(Vector2 pointerPosition)
    {
        Quaternion rotation = Quaternion.Euler(pointerPosition.x,pointerPosition.y,0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
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
        Vector2 mousePosition = Input.mousePosition;
        Debug.Log(mousePosition);
        return mousePosition;
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
