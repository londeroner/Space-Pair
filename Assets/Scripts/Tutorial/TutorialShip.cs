using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialShip : MonoBehaviour
{
    public ShipStats Stats;

    public List<AttackModule> AttackModules = new List<AttackModule>();
    public List<DefenseModule> DefenseModules = new List<DefenseModule>();
    public List<EnergyModule> EnergyModules = new List<EnergyModule>();

    public List<ApplicableEffect> StatModifiers = new List<ApplicableEffect>();

    public bool IsEnemyShip = true;
    public Transform SpawnShipPoint;

    [HideInInspector] public bool Destroyed = false;

    private void Awake()
    {
        // TEMP Before RPG system implement
        Stats.CurrentHP = Stats.MaxHP;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public Damage CalculateDamage(bool shooting = true)
    {
        float energy = 0f, kinetic = 0f;
        float energyAdd = 0f, energyMult = 1f, kineticAdd = 0f, kineticMult = 1f;
        float dmgAdd = 0f, dmgMult = 1f;

        foreach (var effect in StatModifiers)
        {
            dmgAdd += effect.StatModifier.DamageInc;
            dmgMult *= effect.StatModifier.DamageMult != 0 ? effect.StatModifier.DamageMult : 1;

            energyAdd += effect.StatModifier.EnergyDmgInc;
            energyMult *= effect.StatModifier.EnergyDmgMult != 0 ? effect.StatModifier.EnergyDmgMult : 1;

            kineticAdd += effect.StatModifier.KineticDmgInc;
            kineticMult *= effect.StatModifier.KineticDmgMult != 0 ? effect.StatModifier.KineticDmgMult : 1;
        }

        foreach (var module in AttackModules)
        {
            if (!shooting || module.EnergyConsumption <= Stats.CurrentEnergy)
            {
                Stats.CurrentEnergy -= shooting ? module.EnergyConsumption : 0f;
                switch (module.DamageType)
                {
                    case DamageType.Kinetic:
                        kinetic += (module.Damage + kineticAdd + dmgAdd) * dmgMult * kineticMult;
                        break;
                    case DamageType.Energy:
                        energy += (module.Damage + energyAdd + dmgAdd) * dmgMult * energyMult;
                        break;
                }
            }
            else StartCoroutine(GetComponent<TutorialShipEffects>().ShowNoEnergy());
        }

        var result = new Damage(kinetic, energy);

        GetComponent<TutorialShipEffects>().SetAttackText(result);
        return result;
    }

    public void CalculateMaxEnergy(bool refillToFull = true)
    {
        float maxEnergy = 0f;

        foreach (var module in EnergyModules)
        {
            maxEnergy += module.MaxEnergy;
        }

        Stats.MaxEnergy = maxEnergy;

        if (refillToFull)
            Stats.CurrentEnergy = maxEnergy;
    }

    public void RefillEnergy()
    {
        float addedEnergy = 0f, multEnergy = 1f;

        foreach (var effect in StatModifiers)
        {
            addedEnergy += effect.StatModifier.EnergyInc;
            multEnergy *= effect.StatModifier.EnergyMult != 0 ? effect.StatModifier.EnergyMult : 1;
        }

        foreach (var module in EnergyModules)
        {
            Stats.CurrentEnergy += module.EnergyRefill;
        }

        CalculateMaxEnergy(false);
        Stats.CurrentEnergy = Mathf.Min(Stats.CurrentEnergy, Stats.MaxEnergy);
    }

    public void Attack(TutorialShip target, Damage damage)
    {
        if (damage.Kinetic <= 0 && damage.Energy <= 0)
        {
            TutorialGameManager.instance.GameState = GameState.NoAction;
            return;
        }

        var bullet = Instantiate(TutorialGameManager.instance.BulletPrefab, SpawnShipPoint.position, Quaternion.Euler(0,90,0));

        bullet.GetComponent<TutorialProjectile>().SetupProjectile(gameObject, target.gameObject, damage, target.SpawnShipPoint.position);
    }

    public void Defense()
    {
        float es = 0f, avgRatio = -1f;

        float addedEs = 0f, multEs = 1f;

        foreach (var effect in StatModifiers)
        {
            addedEs += effect.StatModifier.ESInc;
            multEs *= effect.StatModifier.ESMult != 0 ? effect.StatModifier.ESMult : 1;
        }

        foreach (var module in DefenseModules)
        {
            if (module.EnergyConsumption <= Stats.CurrentEnergy)
            {
                Stats.CurrentEnergy -= module.EnergyConsumption;

                Stats.Armour += module.DefenceEffect.BonusArmour;
                es += (module.DefenceEffect.MaxES + addedEs) * multEs;

                if (module.CanReflect)
                {
                    Stats.CanReflect = true;
                    if (avgRatio == -1)
                        avgRatio = module.ReflectDamageRatio;
                    else avgRatio = (avgRatio + module.ReflectDamageRatio) / 2;
                }
            }
            else StartCoroutine(GetComponent<TutorialShipEffects>().ShowNoEnergy());
        }

        if (avgRatio != -1)
            Stats.ReflectRatio = avgRatio;
        if (Stats.ES < es) Stats.ES = es;
    }

    public void TakeDamage(Damage amount)
    {
        if (amount.Energy > Stats.ES)
        {
            amount.Energy = amount.Energy - Stats.ES;
            Stats.ES = 0;
            Stats.CurrentHP = Mathf.Max(0f, Stats.CurrentHP - amount.Energy);
        }
        else Stats.ES = Stats.ES - amount.Energy;

        if (amount.Kinetic > Stats.Armour)
        {
            amount.Kinetic = amount.Kinetic - Stats.Armour;
            Stats.Armour = 0;
            Stats.CurrentHP = Mathf.Max(0f, Stats.CurrentHP - amount.Kinetic);
        }
        else Stats.Armour = Stats.Armour - amount.Kinetic;

        CheckDestroyed();
    }

    public void Reflect(Damage damage, TutorialShip target)
    {
        Stats.CanReflect = false;
        Attack(target, new Damage(damage.Kinetic * Stats.ReflectRatio, damage.Energy * Stats.ReflectRatio));
    }

    public void CheckDestroyed()
    {
        //TODO FINISH GAME
        if (Stats.CurrentHP <= 0)
        {
            // TEMP TEST GameManager.instance.IsGameFinished = true;
            Destroyed = true;

            if (TutorialManager.instance.TutorialSteps >= TutorialSteps.PiratesFight1 && TutorialManager.instance.TutorialSteps <= TutorialSteps.PiratesEnd)
            {
                TutorialManager.instance.TutorialSteps = TutorialSteps.PiratesEnd;
            }

            TutorialManager.instance.TutorialStep();

            Destroy(gameObject);
        }
    }

    public void AddArmour(float amount)
    {
        Stats.Armour += amount;
    }
}