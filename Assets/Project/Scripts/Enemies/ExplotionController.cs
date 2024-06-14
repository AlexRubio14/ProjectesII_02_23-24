using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplotionController : MonoBehaviour
{
    [SerializeField]
    private float rangeExplosion;
    [SerializeField] 
    private float damageExplosion;
    [SerializeField]
    private LayerMask collideLayer;
    [SerializeField]
    private GamepadRumbleManager.Rumble explosionGamepadRumble;
    private void Start()
    {
        GamepadRumbleManager.Instance.AddRumble(explosionGamepadRumble);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, rangeExplosion, Vector2.zero, 0, collideLayer);
        foreach (RaycastHit2D hit in hits)
        {
            switch (hit.transform.gameObject.tag)
            {
                case "Player":
                    hit.transform.GetComponent<PlayerController>().GetDamage(damageExplosion, transform.position); 
                    break;
                case "Enemy":
                    hit.transform.GetComponent<Enemy>().Die(); 
                    break;
                default:
                    break;
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, rangeExplosion);
    }
}
