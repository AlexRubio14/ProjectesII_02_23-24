using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHelpController : MonoBehaviour
{
    private Rigidbody2D rb2d;

    [Space, Header("Raycasts"), SerializeField]
    private float distance;

    [Space, Header("AutoHelp"), SerializeField]
    private float autoHelpMagnitude;
    [SerializeField]
    private LayerMask mapLayer;

    [SerializeField]
    private int maxDegree;
    [SerializeField]
    private int totalRays;


    public Vector2 autoHelpDirection { private set; get; }


    [Space, SerializeField]
    private bool showGizmos = false;

    private SizeUpgradeController sizeUpgrade;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sizeUpgrade = GetComponent<SizeUpgradeController>();
        //rayCastDegree = maxDegree / maxDegree;
    }

    private void FixedUpdate()
    {
        AutoHelp();
    }

    private void AutoHelp()
    {
        List<RaycastHit2D> rayCastHits = CheckRayCastsAutoHelp();

        ApplyAutoHelp(rayCastHits);

        rayCastHits.Clear();
    }

    private List<RaycastHit2D> CheckRayCastsAutoHelp()
    {
        List<RaycastHit2D> rayCastHits = new List<RaycastHit2D>();

        for (float i = 0; i < maxDegree; i += maxDegree / totalRays)
        {
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);

            rayCastHits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.right).normalized, distance * sizeUpgrade.sizeMultiplyer, mapLayer));
        }
        return rayCastHits;
    }

    private void ApplyAutoHelp(List<RaycastHit2D> _rayCastHits)
    {
        Vector2 autoHelpVector = Vector2.zero;

        foreach(RaycastHit2D hit in _rayCastHits)
        {
            if (!hit)
                continue;

                Vector2 collisionPoint = hit.collider.ClosestPoint(transform.position);
                autoHelpVector += ((Vector2)transform.position - collisionPoint);
        }

        rb2d.AddForce(autoHelpVector * autoHelpMagnitude, ForceMode2D.Impulse);

        autoHelpDirection = autoHelpVector.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.green;
        
        for (float i = 0; i < maxDegree; i += maxDegree / totalRays)
        {
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.right) * distance * sizeUpgrade.sizeMultiplyer);        
        }

    }

}
