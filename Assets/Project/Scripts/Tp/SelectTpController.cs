using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SelectTpController : MonoBehaviour
{
    
    public TpObject tp;

    public Transform tpPosition;
    [Space, SerializeField]
    private bool isDark;
    [SerializeField]
    private float minLightValue;
    private float defaultLightValue;
    private Light2D playerLight;
    private void Start()
    {
        playerLight = PlayerManager.Instance.player.GetComponentInChildren<Light2D>();
        defaultLightValue = playerLight.pointLightOuterRadius;

        if (SelectTpsManager.instance.GetIdToTeleport() == tp.id)
        {
            PlayerManager.Instance.player.transform.position = tpPosition.position;
            CheckTpOnLightZone();
        }

        

    }

    public void CheckTpOnLightZone()
    {
        if (isDark)
            StartCoroutine(ChangeLightValue(minLightValue));
        else
            StartCoroutine(ChangeLightValue(defaultLightValue));

        IEnumerator ChangeLightValue(float _lightValue)
        {
            yield return new WaitForEndOfFrame();
            playerLight.pointLightOuterRadius = _lightValue;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SelectTpsManager.instance.AddDiscoveredTp(tp.id);
            SelectTpsManager.instance.SetIdToTeleport(tp.id);
        }
    }
}
