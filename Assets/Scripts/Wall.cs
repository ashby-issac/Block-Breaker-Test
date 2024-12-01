using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, ICollider
{
    private SpriteRenderer spriteRenderer;
    public bool HasCollided;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
}
