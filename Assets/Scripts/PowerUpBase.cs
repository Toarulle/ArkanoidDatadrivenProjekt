using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    [Tooltip("Falling speed of the power ups")][SerializeField] private float fallSpeed;
    [Tooltip("How many points the power ups give when taken")][SerializeField] private int points;

    void Update()
    {
        MoveDown();
    }

    public abstract void EnablePowerUp();
    
    private void MoveDown()
    {
        transform.Translate(0,-fallSpeed*Time.deltaTime,0);
    }

    public virtual void OnValidate()
    {
        if (fallSpeed < 0)
        {
            Debug.LogWarning("Fall Speed has to be above 0");
            fallSpeed = 0.01f;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Paddle"))
        {
            EnablePowerUp();
            FindObjectOfType<GameBehaviour>().AddPoints(points);
            Destroy(gameObject);
        }

        if (col.CompareTag("Void"))
        {
            Destroy(gameObject);
        }
    }
}
