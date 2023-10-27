using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions : MonoBehaviour
{
    public static List<Vector2> m_eightDirections = new List<Vector2> {
        new Vector2(0, 1).normalized,
        new Vector2(1, 1).normalized,
        new Vector2(1, 0).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(0, -1).normalized,
        new Vector2(-1, -1).normalized,
        new Vector2(-1, 0).normalized,
        new Vector2(-1, 1).normalized,
    };
}
