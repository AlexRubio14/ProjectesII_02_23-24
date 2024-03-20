using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHubMusic : MonoBehaviour
{
    private void Start()
    {
        ShopMusic.instance.PlayMusic();
    }
}
