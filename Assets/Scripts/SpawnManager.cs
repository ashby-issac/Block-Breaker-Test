using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform blocksParent;
    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private Vector2 blockStartPos;
    [SerializeField] private Vector2 blockEndPos;

    private LevelManager levelManager;
    private LevelsInfo levelsInfo;
    private List<string> blockMap;

    void Start()
    {
        levelManager = ServiceLocator.Get<LevelManager>();
        levelsInfo = levelManager.LevelsInfo;
        SpawnBlocks();
    }

    private void SpawnBlocks()
    {
        foreach (var level in levelsInfo.levels)
        {
            if (level.levelNumber == levelManager.LevelNumber)
            {
                blockMap = level.blockMap;
                break;
            }
        }

        Vector2 offsetVector = Vector2.zero, blockPos = Vector2.zero;
        
        for (int i = 0; i < blockMap.Count(); i++)
        {
            var currentBlockMap = blockMap[i].Split(",");
            for (int j = 0; j < currentBlockMap.Length; j++)
            {
                offsetVector = new Vector2(int.Parse($"{currentBlockMap[j]}"), i);
                blockPos = blockStartPos + offsetVector;

                if (blockPos.x > blockEndPos.x || blockPos.y > blockEndPos.y)
                    continue;

                var blockInstance = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)], blockPos,
                    Quaternion.identity);
                blockInstance.transform.SetParent(blocksParent);
                GameManager.Instance.AddToBlocks(blockInstance.GetComponent<Block>());
            }
        }
    }
}