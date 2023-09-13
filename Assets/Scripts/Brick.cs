using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Brick : MonoBehaviour
{
    [Tooltip("The amount of points player gets when damaging the brick (a non-negative value)")][SerializeField] public int points;
    [Tooltip("The amount of points the player gets when destroying the brick (a non-negative value)")][SerializeField] public int destroyPoints;
    [Tooltip("The health the brick has (Should not be higher than the amount of sprites in the Sprite List) (a non-zero, non-negative value)")][SerializeField] private int health = 1;
    [Tooltip("If this brick should have a power up")][SerializeField] private bool hasPowerUp = true;
    [Tooltip("How big the chance of it dropping a power up is (Range 0.0-1.0, 1.0 is 100% chance)")][Range(0.0f,1.0f)][SerializeField] private float powerUpDropChance = 0.1f;
    [Tooltip("What power ups should the brick be able to drop")][SerializeField] private List<GameObject> powerUpList = null;
    [Tooltip("What different sprites should the brick have, depending on health. Largest health is the last in the list. " +
             "(This should correspond to the amount of health a brick has. Same amount of sprites as health on the toughest brick")][SerializeField] 
    private Sprite[] spriteList;
    [Tooltip("If the brick should be unbreakable")][SerializeField] private bool unbreakable = false;

    private readonly Vector3 powerUpSpawnLevel = new Vector2(0f,-0.15f);
    private GameBehaviour gb = null;
    private void Start()
    {
        gb = FindObjectOfType<GameBehaviour>();
    }

    private void OnValidate()
    {
        if (points < 0)
        {
            Debug.LogWarning("Points can't be negative", this);
            points = 0;
        }
        if (destroyPoints < 0)
        {
            Debug.LogWarning("Destroy Points can't be negative", this);
            destroyPoints = 0;
        }
        if (health < 1)
        {
            Debug.LogWarning("Health can't be below 1", this);
            health = 1;
        }
    }

    public bool HitBrick(int damage = 1)
    {
        if (unbreakable) return false;
        health -= damage;
        if (health <= 0)
        {
            DestroyBrick();
            gb.AddPoints(destroyPoints);
            gb.RemoveBrickFromPlay(gameObject);
            return true;
        }
        UpdateSprite();
        gb.AddPoints(points);
        return false;
    }

    private void DestroyBrick()
    {
        if (hasPowerUp)
        {
            if (Random.value <= powerUpDropChance)
            {
                Instantiate(powerUpList[Random.Range(0,
                            powerUpList.Count)],
                     transform.position+powerUpSpawnLevel,
            quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    private void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = spriteList[health - 1];
    }
}