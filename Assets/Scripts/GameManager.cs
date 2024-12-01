using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject paddlePrefab;
    [SerializeField] private GameObject ballPrefab;

    [Range(0.1f, 5f)] [SerializeField] float gameSpeed = 1f;
    [SerializeField] int pointsPerBlock = 75;

    [SerializeField] int currentScore = 0;

    // [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] bool isAutoplayEnabled = false;

    public static GameManager Instance;

    private PaddleController paddleController = null;
    private Ball ball = null;
    private Vector2 start, end;

    private List<Wall> walls = new List<Wall>();
    private Wall loseWall;
    
    private List<Block> blocks = new List<Block>();
    private List<Block> closeDistBlocks = new List<Block>();

    private LevelManager levelManager;
    private CollisionsHandler collisionsHandler;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(Instance);
    }

    public void OnLevelStart()
    {
        if (levelManager == null)
            levelManager = ServiceLocator.Get<LevelManager>();
        if (collisionsHandler == null)
            collisionsHandler = ServiceLocator.Get<CollisionsHandler>();

        loseWall = levelManager.LoseWall;
        Debug.Log($"OnLevelStart");
        walls.Clear();
        Debug.Log($"levelManager.Walls: {levelManager.Walls.Count}");
        walls = levelManager.Walls.ToList();

        foreach (Wall wall in walls)
            Debug.Log(wall.name);

        var paddlePrefabInst = SpawnPaddleAndBall(out var ballPrefabInst);
        ball.SetupBallStartPosition(ballPrefabInst, paddlePrefabInst);
    }

    public void AddToBlocks(Block block)
    {
        blocks.Add(block);
    }

    public void RefreshBlocks(Block blockRef)
    {
        blocks.Remove(blockRef);
    }

    private GameObject SpawnPaddleAndBall(out GameObject ballPrefabInst)
    {
        Debug.Log($"SpawnPaddleAndBall");
        Vector3 screenCenter = Camera.main.transform.position;

        var paddlePrefabInst = Instantiate(paddlePrefab, new Vector2(screenCenter.x, 0.5f), Quaternion.identity);
        paddleController = paddlePrefabInst.GetComponent<PaddleController>();

        var ballPos = paddlePrefabInst.transform.position + new Vector3(0, paddlePrefabInst.transform.localScale.y);
        ballPrefabInst = Instantiate(ballPrefab, ballPos, Quaternion.identity);
        ball = ballPrefabInst.GetComponent<Ball>();
        return paddlePrefabInst;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);
    }

    void Update()
    {
        // Debug keys
        if (Input.GetKeyDown(KeyCode.O))
            Time.timeScale = 0;
        else if (Input.GetKeyDown(KeyCode.I))
            Time.timeScale = 1;

        collisionsHandler.DetectBlockCollisions(ball, blocks);
        collisionsHandler.DetectWallCollisions(ball, walls);
        collisionsHandler.DetectLoseWallCollision(ball, loseWall);
        if (!ball.lockToPaddle)
        {
            Debug.Log($"Not Locked");
            collisionsHandler.DetectPaddleCollisions(ball, paddleController);
        }
    }

    public void UpdateCurrentScore()
    {
        currentScore += pointsPerBlock;
    }

    public void ResetGame()
    {
        Destroy(this.gameObject);
    }

    public bool IsAutoplayEnabled() => isAutoplayEnabled;
}