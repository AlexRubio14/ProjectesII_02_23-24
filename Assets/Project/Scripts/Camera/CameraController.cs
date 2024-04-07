
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public GameObject objectToFollow;

    [Space, Header("Camera Shake"), SerializeField]
    private float traumaReduction;
    [SerializeField]
    private float maxShakePosition;
    [SerializeField]
    private float maxShakeRotation;
    private Quaternion starterRotation;
    private float traumaLevel;
    [SerializeField]
    private float maxTraumaLevel = 1;

    [Space, Header("Add Trauma Levels"), SerializeField]
    private float minTraumaAdd;
    [SerializeField]
    private float midTraumaAdd;
    [SerializeField]
    private float maxTraumaAdd;

    private SizeUpgradeController sizeUpgrade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

    }

    private void Start()
    {
        sizeUpgrade = FindObjectOfType<SizeUpgradeController>();

        starterRotation = transform.rotation;
    }

    private void Update()
    {
        UpdateTraumaValues();
    }

    private void UpdateTraumaValues()
    {
        if (traumaLevel > 0)
        {
            traumaLevel -= Time.deltaTime * traumaReduction;
            traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
        }
    }

    private void LateUpdate()
    {
        SetRandomTraumaPosition();
        SetRandomTraumaRotation();      
    }

    private void SetRandomTraumaPosition()
    {
        Vector2 positionShake = new Vector2(
                   Random.Range(-maxShakePosition, maxShakePosition),
                   Random.Range(-maxShakePosition, maxShakePosition)
                   );

        float positionX = Mathf.PerlinNoise(0, Time.fixedTime);
        float positionY = Mathf.PerlinNoise(1, Time.fixedTime);

        float offsetX = maxShakePosition * positionShake.x * positionX * (traumaLevel * traumaLevel);
        float offsetY = maxShakePosition * positionShake.y * positionY * (traumaLevel * traumaLevel);

        transform.position = new Vector3(
            objectToFollow.transform.position.x + offsetX * sizeUpgrade.sizeMultiplyer,
            objectToFollow.transform.position.y + offsetY * sizeUpgrade.sizeMultiplyer,
            transform.position.z
            );
    }

    private void SetRandomTraumaRotation()
    {
        float rotation = Mathf.PerlinNoise(2, Time.fixedTime);
        float rotationShake = Random.Range(-maxShakeRotation, maxShakeRotation);

        float angle = maxShakeRotation * rotationShake * rotation * (traumaLevel * traumaLevel);

        transform.rotation = new Quaternion(
            starterRotation.x,
            starterRotation.y,
            starterRotation.z + angle * sizeUpgrade.sizeMultiplyer,
            starterRotation.w
            );
    }

    public void AddLowTrauma()
    {
        AddCustomTrauma(minTraumaAdd);
    }
    public void AddMediumTrauma()
    {
        AddCustomTrauma(midTraumaAdd);
    }
    public void AddHighTrauma()
    {
        AddCustomTrauma(maxTraumaAdd);
    }

    public void AddCustomTrauma(float _trauma)
    {
        if (traumaLevel < _trauma)
        {
            traumaLevel = _trauma;
            traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
        }
    }
    public void SetTrauma(float _trauma)
    {
        traumaLevel = _trauma;
        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
    }
}
