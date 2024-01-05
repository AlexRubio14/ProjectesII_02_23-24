using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    [SerializeField]
    private Transform cam;
    [SerializeField]
    private float relativeMove;
    [SerializeField]
    private bool lockY; 

    void Update()
    {
        if (lockY)
            transform.position = new Vector2(cam.position.x * relativeMove, transform.position.y);
        else
            transform.position = new Vector2(cam.position.x * relativeMove, cam.position.y * relativeMove);
    }
}
