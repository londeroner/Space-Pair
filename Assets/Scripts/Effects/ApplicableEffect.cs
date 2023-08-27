using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicableEffect
{
    public int Duration;
    public StatModifier StatModifier;

    public ApplicableEffect(int duration, StatModifier statModifier, Ship target)
    {
        Duration = duration * 2 + 1;
        StatModifier = statModifier;

        GameManager.instance.TurnEnd += (e) =>
        {
            if (Duration > -1)
            {
                Duration--;

                if (Duration == 0)
                {
                    target.StatModifiers.Remove(this);
                }
            }
        };

        target.CalculateDamage(false);
    }
}
