using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PaddleController : MonoBehaviour, ICollider
{
    [SerializeField] float minXOffset = 1f;
    [SerializeField] float maxXOffset = 15f;
    [SerializeField] float screenWidthInUnits = 16f;
    
    public float PaddleBallDetectionRange = 7f;
    
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private Ball ball;
    public bool HasCollided;

    private void InitProps(GameManager gameManager, Ball ball)
    {
        this.gameManager = gameManager;
        this.ball = ball;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 paddlePos = new Vector2(transform.position.x, transform.position.y);
        paddlePos.x = Mathf.Clamp(GetXPos(), minXOffset, maxXOffset);  
        transform.position = paddlePos;
    }

    private float GetXPos()
    {
        if (GameManager.Instance.IsAutoplayEnabled())
        {
            return ball.transform.position.x;
        }
        return Input.mousePosition.x / Screen.width * screenWidthInUnits;
    }

    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
    
    public void ChangePaddleState()
    {
        HasCollided = false;
    }
}
