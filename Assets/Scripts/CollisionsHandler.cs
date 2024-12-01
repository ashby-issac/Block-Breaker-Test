using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionsHandler : MonoBehaviour
{
    private Block previousCollidedBlock;
    private Wall previousCollidedWall;
    private PaddleController paddleController;
    
    private void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    public void DetectLoseWallCollision(Ball ball, Wall loseWall)
    {
        if (ball == null || !ball.isActiveAndEnabled || loseWall == null) return;
        
        if (IsColliding(ball.transform.position, ball.WidthInWorldUnits / 2, GetObjectRect(loseWall,
                loseWall.transform)) && !loseWall.HasCollided)
        {
            Debug.Log($" Is Colliding with lose wall ");
            loseWall.HasCollided = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
    }
    
    public void DetectWallCollisions(Ball ball, List<Wall> walls)
    {
        if (previousCollidedWall != null && !IsColliding(ball.transform.position, ball.WidthInWorldUnits / 2,
                GetObjectRect(previousCollidedWall, previousCollidedWall.transform)))
        {
            previousCollidedWall.HasCollided = false;
            previousCollidedWall = null;
        }

        foreach (var wall in walls)
        {
            if (wall != null && IsColliding(ball.transform.position, ball.WidthInWorldUnits / 2,
                                 GetObjectRect(wall, wall.transform))
                             && !wall.HasCollided)
            {
                wall.HasCollided = true;
                previousCollidedWall = wall;
                OnHandleCollision(ball, wall, wall.transform);
                Debug.Log($"-------------------------------------------------");
                return;
            }
        }
    }
    
    public void DetectBlockCollisions(Ball ball, List<Block> blocks)
    {
        if (ball == null || !ball.isActiveAndEnabled) return;

        if (previousCollidedBlock != null && !IsColliding(ball.transform.position, ball.WidthInWorldUnits / 2,
                GetObjectRect(previousCollidedBlock, previousCollidedBlock.transform)))
        {
            previousCollidedBlock.HasCollided = false;
            previousCollidedBlock = null;
        }

        foreach (var block in blocks)
        {
            if ((block != null && block.isActiveAndEnabled) && IsColliding(ball.transform.position,
                                                                ball.WidthInWorldUnits / 2,
                                                                GetObjectRect(block, block.transform))
                                                            && !block.HasCollided)
            {
                Debug.Log($":: HasCollidedBlock: {block.transform.name}");
                block.HandleHits();
                block.HasCollided = true;
                OnHandleCollision(ball, block, block.transform);
                previousCollidedBlock = block;
                Debug.Log($"-------------------------------------------------");
                return;
            }
        }
    }
    
    public void DetectPaddleCollisions(Ball ball, PaddleController paddleController)
    {
        if (ball == null || !ball.isActiveAndEnabled) return;

        if (Vector2.Distance(ball.transform.position, paddleController.transform.position) >
            paddleController.PaddleBallDetectionRange) return;

        if (IsColliding(ball.transform.position, ball.WidthInWorldUnits / 2, GetObjectRect(paddleController,
                paddleController.transform)) && !paddleController.HasCollided)
        {
            Debug.Log($" Is Colliding with paddle ");
            paddleController.HasCollided = true;
            this.paddleController = paddleController;
            Invoke(nameof(ChangePaddleState), 0.4f);

            OnHandleCollision(ball, paddleController, paddleController.transform);
            Debug.Log($"-------------------------------------------------");
            return;
        }
    }

    void ChangePaddleState()
    {
        if (paddleController)
            paddleController.ChangePaddleState();
    }
    
    private void OnHandleCollision(Ball ball, ICollider objRef, Transform objTransform = null)
    {
        Vector2 blockCollisionPoint = default, blockNormalSide = default;

        CalculateCollisionNormalAndPoint(ball.transform.position, objRef, objTransform,
            ref blockCollisionPoint, ref blockNormalSide);

        ball.UpdateBallVelocity(blockCollisionPoint, blockNormalSide);
    }
    
    private void CalculateCollisionNormalAndPoint(Vector2 ballCenter, ICollider collider, Transform objTransform,
        ref Vector2 collisionPointOnBlock, ref Vector2 blockNormalSide)
    {
        Rect blockRect = GetObjectRect(collider, objTransform);

        float closestX = Mathf.Clamp(ballCenter.x, blockRect.xMin, blockRect.xMax);
        float closestY = Mathf.Clamp(ballCenter.y, blockRect.yMin, blockRect.yMax);

        collisionPointOnBlock = new Vector2(closestX, closestY);

        Debug.Log($" collisionPointOnBlock: {collisionPointOnBlock} ");

        if (Mathf.Approximately(closestX, blockRect.xMin))
        {
            blockNormalSide = Vector2.left;
            return;
        }
        else if (Mathf.Approximately(closestX, blockRect.xMax))
        {
            blockNormalSide = Vector2.right;
            return;
        }
        else if (Mathf.Approximately(closestY, blockRect.yMin))
        {
            blockNormalSide = Vector2.down;
            return;
        }
        else if (Mathf.Approximately(closestY, blockRect.yMax))
        {
            blockNormalSide = Vector2.up;
            return;
        }

        if (blockNormalSide != default)
        {
            Debug.Log($" blockNormalSide: {blockNormalSide} ");
            return;
        }

        blockNormalSide = Vector2.zero; // Fallback (shouldn't happen if collision is valid)
    }
    
    private bool IsColliding(Vector2 ballCenter, float ballRadius, Rect blockRect)
    {
        float closePointOnX = Mathf.Clamp(ballCenter.x, blockRect.xMin, blockRect.xMax);
        float closePointOnY = Mathf.Clamp(ballCenter.y, blockRect.yMin, blockRect.yMax);

        float distX = ballCenter.x - closePointOnX;
        float distY = ballCenter.y - closePointOnY;

        return (distX * distX) + (distY * distY) <= (ballRadius * ballRadius);
    }
    
    private Rect GetObjectRect(ICollider objRef, Transform objTransform)
    {
        Vector3 blockPosition = objTransform.position;

        if (objRef.GetSpriteRenderer() == null)
            return new Rect();

        Vector2 blockSize = objRef.GetSpriteRenderer().bounds.size;

        return new Rect(
            blockPosition.x - blockSize.x / 2,
            blockPosition.y - blockSize.y / 2,
            blockSize.x,
            blockSize.y
        );
    }
}
