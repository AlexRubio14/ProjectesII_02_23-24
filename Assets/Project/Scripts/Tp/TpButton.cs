using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpButton : MonoBehaviour
{
    private int id;

    public void SetId(int value)
    {
        id = value;
    }

    public int GetId()
    {
        return id;
    }
}
