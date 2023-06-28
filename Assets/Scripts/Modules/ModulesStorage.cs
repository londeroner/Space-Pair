using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModulesStorage : ScriptableObject
{
    public static ModulesStorage instance;

    public List<AttackModule> AllAttackModules;

    public AttackModule GetAttackModule(string name) 
    {
        return AllAttackModules.Find(i => i.Name == name);
    }

    public static ModulesStorage GetStorage()
    {
        if (!instance)
            instance = Resources.Load("ModulesStorage") as ModulesStorage;

        return instance;
    }

}
