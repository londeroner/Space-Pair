using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardStatModifier : MonoBehaviour
{
    public EffectStatModifier effect;
    public Sprite Icon;
    public TMP_Text Description;

    void Start()
    {
        transform.Find("Canvas/Icon").gameObject.GetComponent<Image>().sprite = Icon;

        StringBuilder description = new StringBuilder();

        if (effect.StatModifier.DamageInc != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.DamageInc)][GlobalSettings.instance.Language]} " +
                $"+{effect.StatModifier.DamageInc} ");

        if (effect.StatModifier.DamageMult != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.DamageMult)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.DamageMult} ");

        if (effect.StatModifier.KineticDmgInc != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.KineticDmgInc)][GlobalSettings.instance.Language]} " +
                $"+{effect.StatModifier.KineticDmgInc} ");

        if (effect.StatModifier.KineticDmgMult != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.KineticDmgMult)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.KineticDmgMult} ");

        if (effect.StatModifier.EnergyDmgInc != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.EnergyDmgInc)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.EnergyDmgInc} ");

        if (effect.StatModifier.EnergyDmgMult != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.EnergyDmgMult)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.EnergyDmgMult} ");

        if (effect.StatModifier.ESInc != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.ESInc)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.ESInc} ");

        if (effect.StatModifier.ESMult != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.ESMult)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.ESMult} ");

        if (effect.StatModifier.EnergyInc != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.EnergyInc)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.EnergyInc} ");

        if (effect.StatModifier.EnergyMult != 0)
            description.Append($"{LanguageDictionary.Dict[nameof(StatModifier.EnergyMult)][GlobalSettings.instance.Language]} " +
                $"*{effect.StatModifier.EnergyMult} ");

        Description.text = description.ToString();
    }

    public void ApplyEffect()
    {
        GameManager.instance.PlayerShip.AddEffectStatModifier(effect);
    }
}
