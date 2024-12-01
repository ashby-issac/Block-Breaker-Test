using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, ICollider
{
    [SerializeField] GameObject blockSparklesVFX;
    [SerializeField] Sprite[] spriteHits;
    [SerializeField] int timesHit = 0;

    LevelManager _levelManager;
    SpriteRenderer spriteRenderer;

    public bool HasCollided = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        CountBreakableBlocks();
    }

    private void CountBreakableBlocks()
    {
        _levelManager = GameObject.FindObjectOfType<LevelManager>();
        if (tag == "Breakable")
        {
            _levelManager.CountBlocks();
        }
    }

    public void HandleHits()
    {
        DestroyBlocks();
        return;
        
        timesHit++;
        int maxHits = spriteHits.Length + 1;
        if (timesHit >= maxHits)
        {
            DestroyBlocks();
        }
        else
        {
            ShowNextHitSprite();
        }
    }

    private void ShowNextHitSprite()
    {
        int spriteIndex = timesHit - 1;
        if (spriteHits[spriteIndex] != null)
        {
            GetComponent<SpriteRenderer>().sprite = spriteHits[spriteIndex];
        }
        else
        {
            Debug.LogError("Sprite is missing from " + gameObject.name);
        }
    }

    private void DestroyBlocks()
    {
        GameManager.Instance.UpdateCurrentScore();
        // Destroy(gameObject);
        GameManager.Instance.RefreshBlocks(this);
        gameObject.SetActive(false);
        Destroy(gameObject, 1f);
        _levelManager.DestroyBreakableBlocks();
    }
    

    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
}