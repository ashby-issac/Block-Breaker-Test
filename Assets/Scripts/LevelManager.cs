using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GridSize
{
    public int rows;
    public int columns;
}

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public GridSize gridSize;
    public List<string> blockMap;
}

[System.Serializable]
public class LevelsInfo
{
    public LevelData[] levels;
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int breakableBlocks = 0;
    [SerializeField] private List<Wall> walls = new List<Wall>();
    [SerializeField] private Wall loseWall;
    
    public LevelsInfo LevelsInfo => levelsInfo;
    public int LevelNumber => currentLevel;
    public List<Wall> Walls => walls;
    public Wall LoseWall => loseWall;
    
    private LevelsInfo levelsInfo;
    private SceneLoader sceneLoader;
    
    private void Awake()
    {
        ServiceLocator.Register(this);
        
        var levelTextFile = Resources.Load<TextAsset>("levelData");
        levelsInfo = JsonUtility.FromJson<LevelsInfo>(levelTextFile.text);
    }

    private void Start()
    {
        sceneLoader = ServiceLocator.Get<SceneLoader>();
    }

    public void CountBlocks()
    {
        breakableBlocks++;
    }

    public void DestroyBreakableBlocks()
    {
        breakableBlocks--;
        if (breakableBlocks == 0)
        {
            Invoke("LoadScene", 0.5f);
        }
    }

    private void LoadScene()
    {
        sceneLoader.LoadNextScene();
    }
}