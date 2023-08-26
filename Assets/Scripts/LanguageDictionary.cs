using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageDictionary
{
    public static Dictionary<string, Dictionary<string, string>> Dict 
        = new Dictionary<string, Dictionary<string, string>>();

    static LanguageDictionary()
    {
        Dict.Add(nameof(StatModifier.DamageInc), new Dictionary<string, string>());
        Dict[nameof(StatModifier.DamageInc)].Add("ru", "���������� �����");
        Dict[nameof(StatModifier.DamageInc)].Add("en", "Damage increase");

        Dict.Add(nameof(StatModifier.DamageMult), new Dictionary<string, string>());
        Dict[nameof(StatModifier.DamageMult)].Add("ru", "�������������� �����");
        Dict[nameof(StatModifier.DamageMult)].Add("en", "Damage multiplier");

        Dict.Add(nameof(StatModifier.KineticDmgInc), new Dictionary<string, string>());
        Dict[nameof(StatModifier.KineticDmgInc)].Add("ru", "���������� ������������� �����");
        Dict[nameof(StatModifier.KineticDmgInc)].Add("en", "Kinetic damage increase");

        Dict.Add(nameof(StatModifier.KineticDmgMult), new Dictionary<string, string>());
        Dict[nameof(StatModifier.KineticDmgMult)].Add("ru", "�������������� ������������� �����");
        Dict[nameof(StatModifier.KineticDmgMult)].Add("en", "Kinetic damage multiplier");

        Dict.Add(nameof(StatModifier.EnergyDmgInc), new Dictionary<string, string>());
        Dict[nameof(StatModifier.EnergyDmgInc)].Add("ru", "���������� ��������������� �����");
        Dict[nameof(StatModifier.EnergyDmgInc)].Add("en", "Energy damage increase");

        Dict.Add(nameof(StatModifier.EnergyDmgMult), new Dictionary<string, string>());
        Dict[nameof(StatModifier.EnergyDmgMult)].Add("ru", "�������������� ��������������� �����");
        Dict[nameof(StatModifier.EnergyDmgMult)].Add("en", "Energy damage multiplier");

        Dict.Add(nameof(StatModifier.ESInc), new Dictionary<string, string>());
        Dict[nameof(StatModifier.ESInc)].Add("ru", "���������� ����");
        Dict[nameof(StatModifier.ESInc)].Add("en", "Shield increase");

        Dict.Add(nameof(StatModifier.ESMult), new Dictionary<string, string>());
        Dict[nameof(StatModifier.ESMult)].Add("ru", "��������� ����");
        Dict[nameof(StatModifier.ESMult)].Add("en", "Shield multiplier");

        Dict.Add(nameof(StatModifier.EnergyInc), new Dictionary<string, string>());
        Dict[nameof(StatModifier.EnergyInc)].Add("ru", "���������� �������");
        Dict[nameof(StatModifier.EnergyInc)].Add("en", "Energy increase");

        Dict.Add(nameof(StatModifier.EnergyMult), new Dictionary<string, string>());
        Dict[nameof(StatModifier.EnergyMult)].Add("ru", "��������� �������");
        Dict[nameof(StatModifier.EnergyMult)].Add("en", "Energy multiplier");
    }
}
