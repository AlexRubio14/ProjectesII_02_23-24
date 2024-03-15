using UnityEngine;

[CreateAssetMenu(fileName = "Tp", menuName = "ScriptableObjects/Tp")]
public class TpObject : ScriptableObject
{
    [field: SerializeField]
    public int id { get; private set; }

    [field: SerializeField]
    public string zoneName { get; private set; }

    [field: SerializeField]
    public Sprite zoneImage { get; private set; }

    [field: SerializeField]
    public bool discovered;
}
