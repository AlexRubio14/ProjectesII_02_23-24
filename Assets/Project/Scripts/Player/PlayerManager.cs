using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [field: SerializeField]
    public PlayerController player { get; private set; }

    private void Awake()
    {
        //Si no existe el player que se borre
        if (player == null)
        {
            enabled = false;
            Destroy(this);
            return;

            
        }else if (Instance != null && Instance != this) //Si existe comprobar que no haya una instancia del singletone
        {
            Instance.enabled = false;
            Destroy(Instance);
        }

        Instance = this;
    }
}
