using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBug : EnemyIA
{

    [SerializeField]
    private float timeFollowing = 5.0f; 

    // Start is called before the first frame update
    void Start()
    {
        InitEnemy(); 
    }

    // Update is called once per frame
    void Update()
    {
        Movement(); 
        if(isFollowing)
        {
            Invoke("Die", timeFollowing); 
        }
    }

    private void Die()
    {
        Debug.Log("Exploteee"); 
    }
}
