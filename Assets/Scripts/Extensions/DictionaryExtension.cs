using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtension
{
    public static bool IsDictionaryFilled(this Dictionary<PictureObject, (int, int)> dict)
    {
        bool result = true;

        foreach (var pair in dict)
        {
            if (pair.Value.Item1 != pair.Value.Item2)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    public static bool IsIndexAvailable(this Dictionary<PictureObject, (int, int)> dict, PictureObject obj)
    {
        return dict[obj].Item2 != dict[obj].Item1;
    }

    public static void InitDictionary(this Dictionary<PictureObject, (int, int)> dict, List<SpawnObject> spawnObjects)
    {
        foreach (var obj in spawnObjects)
        {
            dict.Add(obj.PictureObject, (obj.MaxCount, 0));
        }
    }

    public static void IncDictionary(this Dictionary<PictureObject, (int, int)> dict, PictureObject obj)
    {
        var temp = dict[obj];
        temp.Item2++;
        dict[obj] = temp;
    }
}
