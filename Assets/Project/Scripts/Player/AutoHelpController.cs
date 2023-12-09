using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHelpController : MonoBehaviour
{
    private Rigidbody2D c_rb;

    [Space, Header("Raycasts"), SerializeField]
    private float distance;

    [Space, Header("AutoHelp"), SerializeField]
    private float autoHelpMagnitude;
    [SerializeField]
    private LayerMask mapLayer;

    [SerializeField]
    int maxDegree;

    [SerializeField]
    int degreeJump;

    int rayCastDegree;

    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();

        rayCastDegree = maxDegree / degreeJump;
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

        for (int i = 0; i < maxDegree; i += rayCastDegree)
        {
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);

            rayCastHits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.right).normalized, distance, mapLayer));
            if (i != 0)
            {
                direction = transform.rotation * Quaternion.Euler(0, 0, -i);
                rayCastHits.Add(Physics2D.Raycast(transform.position, (direction * Vector2.right).normalized, distance, mapLayer));
            }
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
                autoHelpVector += ((Vector2)transform.position - collisionPoint).normalized;
        }

        c_rb.AddForce(autoHelpVector * autoHelpMagnitude * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        for (float i = 0; i < maxDegree; i += maxDegree / degreeJump)
        {
            Gizmos.color = Color.magenta;
            Quaternion direction = transform.rotation * Quaternion.Euler(0, 0, i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.right) * distance);

            Gizmos.color = Color.magenta;
            direction = transform.rotation * Quaternion.Euler(0, 0, -i);
            Gizmos.DrawLine(transform.position, transform.position + (direction * Vector2.right) * distance);

        }

    }

}
