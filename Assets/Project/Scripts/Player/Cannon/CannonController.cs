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

    [SerializeField]
    private float shootFuel;

    public bool canShoot { get; set; }

    private InputController iController;
    private PlayerController playerController;

    private void Awake()
    {
        iController = GetComponentInParent<InputController>();
        playerController = GetComponentInParent<PlayerController>();
        canShoot = false;
    }
    private void Update()
    {
        Movement(GetMousePosition());

        currentDelay += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Shoot();
    }
    private void Movement(Vector2 pointerPosition)
    {
        //Vector2 direction = (pointerPosition - (Vector2)transform.position).normalized;

        //transform.up = direction;
        
        if(iController.inputAimTurretDirection.normalized == Vector2.zero)
        {
            return;
        }
        transform.up = iController.inputAimTurretDirection.normalized;


    }

    Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return mousePosition;
    }

    public void Shoot()
    {
        if (currentDelay >= reloadDelay && canShoot)
        {
            currentDelay = 0;
            Instantiate(laserPrefab, posToSpawnBullets.position, transform.rotation);
            CameraController.Instance.AddLowTrauma();
            playerController.SubstractHealth(shootFuel);
        }
    }

    
}
