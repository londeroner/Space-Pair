using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : ScriptableObject
{
    public string Name;
    public float EnergyConsumption;

    [HideInInspector]
    public ModuleType ModuleType;
}

[System.Serializable]
public enum ModuleType
{
    NoType = 0,
    AttackModule = 1,
    DefenceModule = 2,
    UtilityModule = 3,
    EnergyModule = 4
}