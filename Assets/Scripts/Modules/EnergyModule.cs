using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Energy Module", menuName = "Modules/Energy Module")]
public class EnergyModule : Module
{
    public float MaxEnergy;
    public float EnergyRefill;

    void Awake()
    {
        ModuleType = ModuleType.EnergyModule;
    }
}
