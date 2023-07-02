using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defense Module", menuName = "Modules/Defense Module")]
public class DefenseModule : Module
{
    public DefenseEffect DefenceEffect;

    public bool CanReflect;

    [Range(0f, 2f)]
    public float ReflectDamageRatio;

    private void Awake()
    {
        ModuleType = ModuleType.DefenceModule;
    }
}

[System.Serializable]
public struct DefenseEffect
{
    public int MaxES;
    public int BonusArmour;
}