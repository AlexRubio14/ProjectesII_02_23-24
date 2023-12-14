using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField]
    private GameObject c_objectToFollow;

    [Space, Header("Camera Shake"), SerializeField]
    private float maxShakePosition;
    [SerializeField]
    private float maxShakeRotation;
    private Quaternion starterRotation;
    private float traumaLevel;
    [SerializeField]
    private float maxTraumaLevel = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

    }

    private void Start()
    {
        starterRotation = transform.rotation;
    }

    private void Update()
    {
        traumaLevel -= Time.deltaTime / 1.5f;

        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);

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
            c_objectToFollow.transform.position.x + offsetX,
            c_objectToFollow.transform.position.y + offsetY,
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
            starterRotation.z + angle,
            starterRotation.w
            );
    }

    public void AddLowTrauma()
    {
        traumaLevel += 0.4f;
        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
    }
    public void AddMediumTrauma()
    {
        traumaLevel += 0.6f;
        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
    }
    public void AddHighTrauma()
    {
        traumaLevel += 1f;
        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
    }

    public void AddCustomTrauma(float _trauma)
    {
        traumaLevel += _trauma;
        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
    }
    public void SetTrauma(float _trauma)
    {
        traumaLevel = _trauma;
        traumaLevel = Mathf.Clamp(traumaLevel, 0, maxTraumaLevel);
    }
}
