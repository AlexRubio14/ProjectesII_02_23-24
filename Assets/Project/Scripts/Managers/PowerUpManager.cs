using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    public enum PowerUpType {NONE, FUEL, ARMOR, DAMAGE }


    public float Armor { get; private set; } // Esta variable se usara para dividir el danyo recibido
    public float Damage { get; private set; } // Esta variable se usara par multiplicar el danyo hecho
    public float Fuel { get; private set; } // Esta variable se usa para sumar la cantidad de fuel total

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
        DontDestroyOnLoad(Instance);

        Armor = 1;
        Damage = 1;
        Fuel = 0;

    }


    public void PowerUpObtained(PowerUpType _powerUpType) 
    {
        switch (_powerUpType)
        {
            case PowerUpType.FUEL:
                Fuel += 100;
                break;
            case PowerUpType.ARMOR:
                Armor += 0.5f;
                break;
            case PowerUpType.DAMAGE:
                Damage += 0.7f;
                break;
            default:
                break;
        }
    }
}
