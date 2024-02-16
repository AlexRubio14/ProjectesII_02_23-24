using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlinkEffect : MonoBehaviour
{
    [Range(0, 10), SerializeField]
    public float speed = 2.0f;

    [SerializeField]
    public Color initColor = Color.white;

    [SerializeField]
    public Color endColor;

    private Image imageComp;
    private Color startColor; 
    [field: SerializeField]
    public bool canBlink { get; set; }
    void Awake()
    {
        imageComp = GetComponent<Image>();
        startColor = imageComp.color;
    }

    void Update()
    {
        if (canBlink)
        {
            imageComp.color = Color.Lerp(initColor, endColor, Mathf.PingPong(Time.time * speed, 1));
        }
        else
        {
            imageComp.color = startColor;
        }
    }
}
