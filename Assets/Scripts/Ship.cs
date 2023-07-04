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

    public bool Destroyed = false;

    void Start()
    {
        Stats.CurrentHP = Stats.MaxHP;
    }

    void Update()
    {
    }

    public IEnumerator Attack(Ship target, int? defaultDamage = null)
    {
        float damage = 0f;

        if (defaultDamage is null)
        {
            foreach (var module in Modules)
            {
                if (module.ModuleType == ModuleType.AttackModule)
                {
                    damage += (module as AttackModule).Damage;
                }
            }
        }
        else damage = defaultDamage.Value;

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
                AddArmour(def.DefenceEffect.BonusArmour);
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
        SetES(es);

        GameManager.instance.GameState = GameState.NoAction;
    }

    public void TakeDamage(float amount)
    {
        if (amount > Stats.ES)
        {
            amount -= Stats.ES;
            SetES(0);
            Stats.CurrentHP = Mathf.Max(0f, Stats.CurrentHP - amount);
        }
        else SetES(Stats.ES - amount);

        CheckDestroyed();
    }

    public void Reflect(float inputDamage, Ship target)
    {
        Stats.CanReflect = false;
        StartCoroutine(Attack(target, (int?)(inputDamage * Stats.ReflectRatio)));
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

    private void SetES(float amount)
    {
        Stats.ES = amount;
    }
    
    private void AddArmour(float amount)
    {
        Stats.Armour += amount;
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