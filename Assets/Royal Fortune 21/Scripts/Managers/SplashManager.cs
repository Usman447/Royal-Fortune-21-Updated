using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    void Start()
    {
        Invoke("LoadScene", 3f);
        PlayerPrefs.SetInt("BonusEnable", 0);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
