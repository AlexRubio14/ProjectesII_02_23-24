using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAData : MonoBehaviour
{
    public List<Transform> m_targets = null;
    public Collider2D[] m_obstacles = null;

    public Transform m_currentTarget;

    public int GetTargetsCount() => m_targets == null ? 0 : m_targets.Count;
}
