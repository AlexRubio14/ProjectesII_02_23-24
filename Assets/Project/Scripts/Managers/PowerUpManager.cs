using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    public enum PowerUpType {NONE, FUEL, DAMAGE }


    [SerializeField]
    private string damageKey;
    public float Damage { get; private set; } // Esta variable se usara par multiplicar el danyo hecho
    [SerializeField]
    private float damageAddition;
    [Space, SerializeField]
    private string fuelKey;
    public float Fuel { get; private set; } // Esta variable se usa para sumar la cantidad de fuel total
    [SerializeField]
    private float fuelAddition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        if (PlayerPrefs.HasKey(fuelKey))
        {
            Damage = PlayerPrefs.GetFloat(damageKey);
            Fuel = PlayerPrefs.GetFloat(fuelKey);
        }
        else
        {
            ResetPowerUps();
        }

    }

    public void PowerUpObtained(PowerUpType _powerUpType) 
    {
        switch (_powerUpType)
        {
            case PowerUpType.FUEL:
                Fuel += fuelAddition;
                PlayerPrefs.SetFloat(fuelKey, Fuel);
                break;
            case PowerUpType.DAMAGE:
                Damage += damageAddition;
                PlayerPrefs.SetFloat(damageKey, Damage);

                break;
            default:
                break;
        }
        
        PlayerPrefs.Save();

    }

    public void ResetPowerUps()
    {
        Damage = 1;
        Fuel = 0;
        PlayerPrefs.SetFloat(damageKey, Damage);
        PlayerPrefs.SetFloat(fuelKey, Fuel);
        PlayerPrefs.Save();
    }
}
