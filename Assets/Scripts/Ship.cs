using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    public ShipStats Stats;

    public List<Module> Modules = new List<Module>();

    [HideInInspector] public bool Destroyed = false;

    void Start()
    {
        Stats.CurrentHP = Stats.MaxHP;
    }

    void Update()
    {
    }

    public Damage CalculateDamage()
    {
        float energy = 0f, kinetic = 0f;

        foreach (var module in Modules)
        {
            if (module.ModuleType == ModuleType.AttackModule)
            {
                switch ((module as AttackModule).DamageType)
                {
                    case DamageType.Kinetic:
                        kinetic += (module as AttackModule).Damage;
                        break;
                    case DamageType.Energy:
                        energy += (module as AttackModule).Damage;
                        break;
                }
            }
        }

        return new Damage(kinetic, energy);

    }

    public IEnumerator Attack(Ship target, Damage damage)
    {
        var bullet = Instantiate(GameManager.instance.BulletPrefab, transform);

        while (bullet.transform.position != target.transform.position)
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, target.transform.position, 7f * Time.deltaTime);
            yield return 0;
        }

        Destroy(bullet);

        target.TakeDamage(damage);

        if (!target.Destroyed && target.Stats.CanReflect)
        {
            target.Reflect(damage, this);
        }
        else GameManager.instance.GameState = GameState.NoAction;
    }

    public void Defense()
    {
        float es = 0f, avgRatio = -1f;
        foreach (var module in Modules)
        {
            if (module.ModuleType == ModuleType.DefenceModule)
            {
                var def = (module as DefenseModule);
                Stats.Armour += def.DefenceEffect.BonusArmour;
                es += def.DefenceEffect.MaxES;

                if (def.CanReflect)
                {
                    Stats.CanReflect = true;
                    if (avgRatio == -1)
                        avgRatio = def.ReflectDamageRatio;
                    else avgRatio = (avgRatio + def.ReflectDamageRatio) / 2;
                }
            }
        }
        Stats.ReflectRatio = avgRatio;
        Stats.ES = es;

        GameManager.instance.GameState = GameState.NoAction;
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
        StartCoroutine(Attack(target, new Damage(damage.Kinetic * Stats.ReflectRatio, damage.Energy * Stats.ReflectRatio)));
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