using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ImageBlinkEffect : MonoBehaviour
{
    [Range(0, 10), SerializeField]
    private float speed = 1.0f;

    [Range(1, 1.5f), SerializeField]
    private float maxExpand = 1.1f;

    [Range(1, 1.5f), SerializeField]
    private float minExpand = 1.02f;

    private Vector3 scaleComp;
    public bool isSelected { private get; set; } 

    void Awake()
    {
        scaleComp = transform.localScale;
        isSelected = false;
    }

    void Update()
    {
        if (isSelected)
        {
            transform.localScale = Vector3.Lerp(scaleComp, scaleComp * maxExpand, Mathf.PingPong(Time.time * speed, 1));
        }
        else 
        { 
            transform.localScale = Vector3.Lerp(scaleComp, scaleComp * minExpand, Mathf.PingPong(Time.time * speed, 1));
        }
    }
}
