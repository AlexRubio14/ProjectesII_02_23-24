using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ImageFloatEffect : MonoBehaviour
{
    [Range(0, 10), SerializeField]
    private float speed = 1.0f;

    [Range(1, 2f), SerializeField]
    private float maxExpand = 1.1f;

    [Range(0.2f, 1.5f), SerializeField]
    private float minExpand = 1.02f;

    private Vector3 scaleComp;
    public bool canFloat { private get; set; } 

    void Awake()
    {
        scaleComp = transform.localScale;
        canFloat = false;
    }

    void Update()
    {
        if (canFloat)
        {
            transform.localScale = Vector3.Lerp(scaleComp * minExpand, scaleComp * maxExpand, Mathf.PingPong(Time.time * speed, 1));
        }
        else
        {
            transform.localScale = scaleComp;
        }
    }
}
