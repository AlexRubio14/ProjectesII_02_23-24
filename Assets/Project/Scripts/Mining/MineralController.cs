using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralController : MonoBehaviour
{
    [field : SerializeField]
    public ItemObject c_currentItem { private set; get; }

    [field : SerializeField]
    public short MaxItemsToReturn { private set; get; }


    private SpriteRenderer c_spriteR;
    private void Awake()
    {
        c_spriteR = GetComponent<SpriteRenderer>();
        c_spriteR.sprite = c_currentItem.c_sprite;
    }

}
