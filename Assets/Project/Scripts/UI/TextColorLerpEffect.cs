using TMPro;
using UnityEngine;

public class TextColorLerpEffect : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textColor;

    [Space, Range(0, 10), SerializeField]
    public float speed = 2.0f;

    [SerializeField]
    public Color startColor;

    [SerializeField]
    public Color endColor;

    [field: SerializeField]
    public bool canLerp { get;  set; }

    void Update()
    {
        if (canLerp)
        {
            textColor.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
        }
        else
        {
            textColor.color = startColor;
        }
    }

}
