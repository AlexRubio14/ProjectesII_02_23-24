using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBug : MonoBehaviour
{
    [SerializeField]
    private EnemyIA enemyIA; 

    [SerializeField]
    private float timeFollowing = 5.0f; 

    // Start is called before the first frame update
    void Start()
    {
        enemyIA.InitEnemy(); 
    }

    // Update is called once per frame
    void Update()
    {
        enemyIA.Movement(); 
        if(enemyIA.isFollowing)
        {
            Invoke("Die", timeFollowing); 
        }
    }

    private void Die()
    {
        Debug.Log("Exploteee"); 
        Destroy(gameObject);
    }
}
