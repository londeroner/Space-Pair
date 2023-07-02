using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack Module", menuName = "Modules/Attack Module")]
public class AttackModule : Module
{
    public DamageType DamageType;
    public float Damage;

    void Awake()
    {
        ModuleType = ModuleType.AttackModule;    
    }
}

[System.Serializable]
public enum DamageType
{
    Kinetic = 0,
    Energy = 1
}