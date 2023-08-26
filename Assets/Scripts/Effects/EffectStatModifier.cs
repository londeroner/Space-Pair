using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectStatModifier", menuName = "Effects/StatModifier")]
public class EffectStatModifier : BaseEffect
{
    public StatModifier StatModifier;
}

[System.Serializable]
public struct StatModifier
{
    public float DamageInc;
    public float DamageMult;
    public float KineticDmgInc;
    public float KineticDmgMult;
    public float EnergyDmgInc;
    public float EnergyDmgMult;
    public float ESInc;
    public float ESMult;
    public float EnergyInc;
    public float EnergyMult;
}