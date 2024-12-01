using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public interface ICollider // Rename
{
    SpriteRenderer GetSpriteRenderer();
}

public class Ball : MonoBehaviour
{
    // config params
    [SerializeField] private PaddleController paddle1;
    [SerializeField] private float xVelocityOffset = 2f;
    [SerializeField] private float yVelocityOffset = 10f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float bounceRandomFactor = 0.2f;
    [SerializeField] private bool hasStarted = false;
    [SerializeField] private float paddleLockTimer = 1f;

    [SerializeField] private Vector2 moveDirection = Vector2.zero;

    private bool canMove = false;
    private Vector2 ballVelocity;
    private Vector2 paddleToBallVector;
    private SpriteRenderer spriteRenderer;
    private Sprite ballSprite;

    public float WidthInWorldUnits, HeightInWorldUnits;
    public bool lockToPaddle = true;
    public float DetectionRadius = 10f;

    public void SetupBallStartPosition(GameObject ballPrefabInst, GameObject paddlePrefabInst)
    {
        var ballBottomCollisionPoint =
            ballPrefabInst.transform.position + (Vector3.down * WidthInWorldUnits / 2);

        var paddleTopCollisionPoint =
            paddlePrefabInst.transform.position + (Vector3.up * paddlePrefabInst.transform.localScale.y / 2);

        if (ballBottomCollisionPoint.y <= paddleTopCollisionPoint.y)
        {
            var pointOverflow = Mathf.Abs(ballBottomCollisionPoint.y - paddleTopCollisionPoint.y);
            ballPrefabInst.transform.position += new Vector3(0, pointOverflow);
        }
    }

    void Start()
    {
        // paddleToBallVector = transform.position - paddle1.transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ballSprite = spriteRenderer.sprite;

        ballVelocity = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
        ballVelocity = moveDirection;
        CovertSpritePixelToWorldUnits();
    }

    private void CovertSpritePixelToWorldUnits()
    {
        // Width and height in world units
        float widthInPixels = ballSprite.rect.width;
        float heightInPixels = ballSprite.rect.height;

        // Convert pixels to world units using pixelsPerUnit
        float pixelsPerUnit = ballSprite.pixelsPerUnit;
        WidthInWorldUnits = widthInPixels / pixelsPerUnit;
        HeightInWorldUnits = heightInPixels / pixelsPerUnit;
    }

    void Update()
    {
        // LockBallToPaddle();
        LauchBall();
    }

    private void LockBallToPaddle()
    {
        Vector2 paddle1Pos = new Vector2(paddle1.transform.position.x, paddle1.transform.position.y);
        transform.position = paddle1Pos + paddleToBallVector;
    }

    private void LauchBall()
    {
        Debug.Log($"LaunchBall");
        if (Input.GetKeyDown(KeyCode.Space) && !canMove)
        {
            Debug.Log($"LaunchBall :: KeyCode.Space");
            canMove = true;
            hasStarted = true;
            Invoke(nameof(OnRemoveLockToPaddle), paddleLockTimer);
        }

        if (canMove)
        {
            Debug.Log($"LaunchBall :: canMove");
            Vector2 updatedPosition = (Vector2)transform.position + ballVelocity * moveSpeed * Time.deltaTime;
            transform.position = updatedPosition;
        }
    }

    void OnRemoveLockToPaddle()
    {
        lockToPaddle = false;
    }

    public void UpdateBallVelocity(Vector2 blockCollisionPoint, Vector2 blockNormalSide)
    {
        var ballPos = (Vector2)transform.position;
        var ballDir = -ballVelocity;
        
        ballVelocity = GetReflectedDir(blockCollisionPoint, ballPos, ballDir, blockNormalSide.normalized);
    }

    private Vector2 vectorProjectionPoint, reflectedPoint;

    private Vector2 GetReflectedDir(Vector2 blockCollisionPoint, Vector2 ballCollisionPoint, Vector2 ballDir,
        Vector2 collisionNormal)
    {
        float scalar = Vector2.Dot(ballDir, collisionNormal);
        var vectorProjectionPoint = scalar * collisionNormal;
        var vecToBallPoint = ((blockCollisionPoint + ballDir * scalar) - (blockCollisionPoint + vectorProjectionPoint));
        var reflectedPoint = (blockCollisionPoint + vectorProjectionPoint) + (-vecToBallPoint);
        
        return (reflectedPoint - blockCollisionPoint).normalized;
    }

    Vector2 Reflect(Vector2 direction, Vector2 normal)
    {
        return direction - 2 * Vector2.Dot(direction, normal) * normal;
    }
}