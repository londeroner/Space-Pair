using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings instance;

    public string Language = "ru";

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;

            var lng = PlayerPrefs.GetString("Language");

            if (!string.IsNullOrEmpty(lng))
            {
                Language = lng;
            }
            else PlayerPrefs.SetString("Language", "ru");
        }
        else Destroy(this);
    }

    public void SetLanguage(string language)
    {
        Language = language;
        PlayerPrefs.SetString("Language", "en");
    }
}
