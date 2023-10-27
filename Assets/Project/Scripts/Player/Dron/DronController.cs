using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private PlayerController controller;

    void Start()
    {
        controller = GetComponentInParent<PlayerController>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 point = controller.transform.position;
        Vector3 axis = new Vector3(0, 0, 1);
        transform.RotateAround(point,axis,speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map"))
        {
            speed = speed * -1;
        }
    }
}
