using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject c_objectToFollow;

    private void LateUpdate()
    {
        transform.position = new Vector3(
            c_objectToFollow.transform.position.x,
            c_objectToFollow.transform.position.y,
            transform.position.z
            );
    }
}
