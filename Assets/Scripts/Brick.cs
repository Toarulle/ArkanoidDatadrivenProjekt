using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Brick : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject powerUp = null;
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private bool unbreakable = false;

    public void HitBrick(int damage = 1)
    {
        if (unbreakable) return;
        health -= damage;
        if (health <= 0)
        {
            DestroyBrick();
        }
        else
        {
            UpdateSprite();
        }
    }

    private void DestroyBrick()
    {
        Destroy(gameObject);
    }

    private void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = spriteList[health - 1];
    }
}