using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindColumnsController : MonoBehaviour
{
    [SerializeField]
    UpgradeObject neededUpgrade;
    AreaEffector2D effector;

    [SerializeField]
    private float forceWithUpgrade;
    [SerializeField]
    private float forceWithoutUpgrade;

    private void Awake()
    {
        effector = GetComponent<AreaEffector2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(UpgradeManager.Instance.CheckObtainedUpgrade(neededUpgrade)) 
        {
            effector.forceMagnitude = forceWithUpgrade;
        }
        else
        {
            effector.forceMagnitude = forceWithoutUpgrade;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
