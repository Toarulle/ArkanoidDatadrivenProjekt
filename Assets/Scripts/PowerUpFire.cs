using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpFire : PowerUpBase
{
    [Tooltip("How much damage the ball does with the power up (min: larger than current damage)")][SerializeField] private int powerUpDamage = 10;
    [Tooltip("Amount of time the power up should last (req. min: 1)")][SerializeField] private int powerUptime = 5;

    private int originalBallDamage;
    void Start()
    {
        originalBallDamage = FindObjectOfType<Ball>().damage;
    }
    
    public override void EnablePowerUp()
    {
        FindObjectOfType<GameBehaviour>().PowerUpFireBall(powerUpDamage, powerUptime);
    }

    public override void OnValidate()
    {
        base.OnValidate();
        
        if (powerUpDamage <= originalBallDamage)
        {
            Debug.LogWarning("Power Up Damage needs to be larger than the current damage of the ball");
            powerUpDamage = originalBallDamage + 1;
        }
        if (powerUptime < 1)
        {
            Debug.LogWarning("Power Up Time needs to be larger than 0");
            powerUptime = 1;
        }
    }
}
