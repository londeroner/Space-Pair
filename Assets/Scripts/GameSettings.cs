using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public int ObjectsNumber = 10;

    public string GameSceneName = "";

    public static GameSettings instance;



    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else Destroy(this);
    }

    public void SetPairNumber(int number)
    {
        if (number % 2 == 1)
            number--;

        ObjectsNumber = number;

        SceneManager.LoadScene(GameSceneName);
    }

    public string GetMaterialDirectoryName()
    {
        return "Materials/";
    }

    public string GetTextureDirectoryName()
    {
        return $"Graphics/PuzzleCat/Fruits/";
    }
}
