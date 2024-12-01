using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int finalSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (currentSceneIndex < finalSceneIndex)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
            GameManager.Instance.ResetGame();
        }
    }

    private void Start()
    {
        OnLevelLoad();
    }

    void OnLevelLoad()
    {
        GameManager.Instance.OnLevelStart();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        ServiceLocator.ClearServices();
    }
}
