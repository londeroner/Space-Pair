using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviour : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (sceneName == "Exit")
            Application.Quit();
        else SceneManager.LoadScene(sceneName);
    }
}
