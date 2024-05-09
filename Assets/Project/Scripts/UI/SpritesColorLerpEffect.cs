using UnityEngine;
using UnityEngine.UI;

public class SpritesColorLerpEffect : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] spritesColor;

    [Space, Range(0, 10), SerializeField]
    public float speed = 2.0f;

    [SerializeField]
    public Color startColor;

    [SerializeField]
    public Color endColor;

    [field: SerializeField]
    public bool canLerp { get;  set; }

    private void OnDisable()
    {
        foreach (SpriteRenderer image in spritesColor)
            image.color = startColor;
    }
    void Update()
    {
        Color nextColor = canLerp ? Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1)) : startColor;
        foreach (SpriteRenderer image in spritesColor)
            image.color = nextColor;
    }

}
