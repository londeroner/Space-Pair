using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    public ShipStats Stats;

    public List<Module> Modules = new List<Module>();

    public Image HPBar;
    public TMP_Text ESText;

    void Start()
    {
        ESText.text = Stats.ES.ToString();
    }

    void Update()
    {
    }

    public IEnumerator Attack(Ship target)
    {
        float damage = 0f;

        foreach (var module in Modules)
        {
            if (module.ModuleType == ModuleType.AttackModule)
            {
                damage += (module as AttackModule).Damage;
            }
        }

        var bullet = Instantiate(GameManager.instance.BulletPrefab, transform);

        while (bullet.transform.position != target.transform.position)
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, target.transform.position, 7f * Time.deltaTime);
            yield return 0;
        }

        Destroy(bullet);

        target.TakeDamage(damage);

        GameManager.instance.GameState = GameState.NoAction;
    }

    public void AddES(float amount)
    {
        Stats.ES += amount;
    }

    public void TakeDamage(float amount)
    {
        if (amount > Stats.ES)
        {
            amount -= Stats.ES;
            Stats.ES = 0;
            Stats.CurrentHP = Mathf.Max(0f, Stats.CurrentHP - amount);
        }
        else Stats.ES -= amount;

        HPBar.fillAmount = Stats.CurrentHP / Stats.MaxHP;
        ESText.text = Stats.ES.ToString();

        CheckDestroyed();
    }

    public void CheckDestroyed()
    {
        //TODO
        if (Stats.CurrentHP <= 0)
        {
            GameManager.instance.IsGameFinished = true;
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct ShipStats
{
    public float MaxHP;
    public float CurrentHP;
    public float ES;
    public float Armour;
}