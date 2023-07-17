using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    public ShipStats Stats;

    public List<AttackModule> AttackModules = new List<AttackModule>();
    public List<DefenseModule> DefenseModules = new List<DefenseModule>();
    public List<EnergyModule> EnergyModules = new List<EnergyModule>();

    [HideInInspector] public bool Destroyed = false;

    private void Awake()
    {
        GameManager.instance.BattleStarted += () => CalculateMaxEnergy();

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

        foreach (var module in AttackModules)
        {
            if (!shooting || module.EnergyConsumption <= Stats.CurrentEnergy)
            {
                Stats.CurrentEnergy -= shooting ? module.EnergyConsumption : 0f;
                switch (module.DamageType)
                {
                    case DamageType.Kinetic:
                        kinetic += module.Damage;
                        break;
                    case DamageType.Energy:
                        energy += module.Damage;
                        break;
                }
            }
            else StartCoroutine(GetComponent<ShipEffects>().ShowNoEnergy());
        }

        return new Damage(kinetic, energy);
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
        foreach (var module in EnergyModules)
        {
            Stats.CurrentEnergy += module.EnergyRefill;
        }

        CalculateMaxEnergy(false);
        Stats.CurrentEnergy = Mathf.Min(Stats.CurrentEnergy, Stats.MaxEnergy);
    }

    public void Attack(Ship target, Damage damage)
    {
        if (damage.Kinetic <= 0 && damage.Energy <= 0)
        {
            GameManager.instance.GameState = GameState.NoAction;
            return;
        }

        var bullet = Instantiate(GameManager.instance.BulletPrefab, transform);

        bullet.GetComponent<Projectile>().SetupProjectile(gameObject, target.gameObject, damage);
    }

    public void Defense()
    {
        float es = 0f, avgRatio = -1f;
        foreach (var module in DefenseModules)
        {
            if (module.EnergyConsumption <= Stats.CurrentEnergy)
            {
                Stats.CurrentEnergy -= module.EnergyConsumption;

                Stats.Armour += module.DefenceEffect.BonusArmour;
                es += module.DefenceEffect.MaxES;

                if (module.CanReflect)
                {
                    Stats.CanReflect = true;
                    if (avgRatio == -1)
                        avgRatio = module.ReflectDamageRatio;
                    else avgRatio = (avgRatio + module.ReflectDamageRatio) / 2;
                }
            }
            else StartCoroutine(GetComponent<ShipEffects>().ShowNoEnergy());
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

    public void Reflect(Damage damage, Ship target)
    {
        Stats.CanReflect = false;
        Attack(target, new Damage(damage.Kinetic * Stats.ReflectRatio, damage.Energy * Stats.ReflectRatio));
    }

    public void CheckDestroyed()
    {
        //TODO
        if (Stats.CurrentHP <= 0)
        {
            GameManager.instance.IsGameFinished = true;
            Destroyed = true;
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct ShipStats
{
    public float MaxHP;
    [HideInInspector] public float CurrentHP;
    public float ES;
    public float Armour;

    [HideInInspector] public float MaxEnergy;
    [HideInInspector] public float CurrentEnergy;

    [HideInInspector] public bool CanReflect;
    [HideInInspector] public float ReflectRatio;
}

[System.Serializable]
public struct Damage
{
    public float Kinetic;
    public float Energy;

    public Damage(float k, float e)
    {
        Kinetic = k;
        Energy = e;
    }
}