using UnityEngine;

public class SizeUpgradeController : MonoBehaviour
{
    [Header("Size Changer"), SerializeField]
    private float smallSize;
    private float normalSize;
    
    [SerializeField]
    private float shrinkSpeed;
    [SerializeField]
    private float growthSpeed;

    private bool growing;
    private float growProcess;
    public float sizeMultiplyer { set; get; }
    [SerializeField]
    private ImageFloatEffect shipSpriteFloatFX;


    [Space, Header("Camera Size"), SerializeField]
    private float smallCamSize;
    private float normalCamSize;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        normalSize = transform.localScale.x;
        growProcess = 1;

        sizeMultiplyer = 1;
        cam = Camera.main;
        normalCamSize = cam.orthographicSize;

        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrowOrShrink(); 
    }

    private void CheckGrowOrShrink()
    {
        if (growing && transform.localScale.x != normalSize)
        {
            ChangeSize(Time.deltaTime, growthSpeed);
            if (growProcess == 0)
                enabled = false;
        }
        else if (!growing && transform.localScale.x != smallSize)
        {
            ChangeSize(-Time.deltaTime, shrinkSpeed);
        }
        else
        {
            shipSpriteFloatFX.canFloat = false;
        }


    }

    private void ChangeSize(float _signedDelta, float _speed)
    {
        shipSpriteFloatFX.canFloat = true;

        growProcess = Mathf.Clamp01(growProcess + _speed * _signedDelta);

        transform.localScale = Vector3.Lerp(Vector3.one * smallSize, Vector3.one * normalSize, growProcess);

        ChangeCamSize();
    }

    private void ChangeCamSize()
    {
        cam.orthographicSize = Mathf.Lerp(smallCamSize, normalCamSize, growProcess);
    }
    public void SetGrowing(bool _growing)
    {
        growing = _growing;

        sizeMultiplyer = growing ? 1 : smallSize;
    }

}
