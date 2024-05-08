using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToFollow;

    [SerializeField]
    private Vector3 offset; 

    // Update is called once per frame
    void Update()
    {
        if(objectToFollow != null)
        {
            transform.position = objectToFollow.transform.position + offset;
        }
    }
}
