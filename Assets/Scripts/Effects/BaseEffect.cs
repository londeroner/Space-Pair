using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : ScriptableObject
{
    public string Name;

    [Tooltip("Effect duration, set -1 for inf")]
    public int Duration;
}
