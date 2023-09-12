using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpBall : PowerUpBase
{
    [Tooltip("The amount of extra balls the power up spawns when caught (min 1)")][SerializeField] private int ballsToSpawn = 3;
    public override void EnablePowerUp()
    {
        FindObjectOfType<GameBehaviour>().SpawnExtraBalls(ballsToSpawn);
    }

    public override void OnValidate()
    {
        base.OnValidate();
        if (ballsToSpawn < 1)
        {
            Debug.LogWarning("Balls To Spawn needs to be larger than 0");
            ballsToSpawn = 1;
        }
    }
}
