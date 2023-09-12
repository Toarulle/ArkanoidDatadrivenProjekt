using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class Ball : MonoBehaviour
{
    [Tooltip("The starting speed the ball has (A non-zero, non-negative value)")] [SerializeField]
    private float speed = 5;
    [Tooltip("How much the ball speed increases with each bounce (not walls) (A non-negative value)")] [SerializeField] private float speedIncrease = 0.1f;

    [Tooltip("How much damage the ball does when it hits a brick (A non-zero, non-negative value)")] public int damage = 1;

    [Tooltip("Original ball sprite")] [SerializeField] private Sprite originalBallSprite;
    [Tooltip("Fireball-power up sprite")] [SerializeField] private Sprite fireBallSprite;

    private AudioController audioController = null;
    private Vector2 direction;
    private int originallySetDamage;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        audioController = FindObjectOfType<AudioController>();
        originallySetDamage = damage;
    }

    private void OnValidate()
    {
        if (speed <= 0.0f)
        {
            Debug.LogWarning("Speed must have a non-zero, non-negative value.", this);
            speed = 0.01f;
        }
        if (damage <= 0)
        {
            Debug.LogWarning("Damage must have a non-zero, non-negative value.", this);
            damage = 1;
        }

        if (speedIncrease < 0.0f)
        {
            Debug.LogWarning("Speed Increase must have a non-negative value.", this);
            speedIncrease = 0.0f;
        }
    }

    public void SendBallFromPaddle()
    {
        float randomDirection = Random.Range(-1f, 1f);
        var newDirection = Vector2.ClampMagnitude(
                             new Vector2(randomDirection, 1), 1);
        rb.velocity = newDirection * speed;
        audioController.HitPaddle();
    }

    public void SendBallFromPowerUp()
    {
        float xDir = Random.Range(-1f, 1f);
        float yDir = Random.Range(-1f, 1f);

        var newDirection = Vector2.ClampMagnitude(new Vector2(xDir, yDir), 1);
        rb.velocity = newDirection * speed;
    }
    
    private float BallDirection(Vector2 racketPosition, Vector2 ballPosition)
    {
        return Math.Clamp(ballPosition.x - racketPosition.x, -1f, 1f);
    }

    public void EnableFireBall(int newDamage)
    {
        damage = newDamage;
        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().sprite = fireBallSprite;
    }
    
    public void DisableFireBall()
    {
        damage = originallySetDamage;
        GetComponent<Collider2D>().isTrigger = false;
        GetComponent<SpriteRenderer>().sprite = originalBallSprite;
    }
    
    private void BallBounce(Vector2 contactPoint)
    {
        var currentVelocity = rb.velocity;
        var newXVelocity = currentVelocity.x;
        var newYVelocity = currentVelocity.y;
        var position = transform.position;
        if (Math.Abs(contactPoint.x - position.x) >
            Math.Abs(contactPoint.y - position.y))
        {
            newXVelocity = currentVelocity.x *-1;
        }else {
            newYVelocity = currentVelocity.y *-1;
        }
        rb.velocity = new Vector2(newXVelocity, newYVelocity);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (transform.parent != null) return;
        
        if(col.collider.CompareTag("BreakableBrick"))
        {
            var destroyed = col.gameObject.GetComponent<Brick>().HitBrick(damage);
            if (destroyed)
            {
                audioController.BreakBrick();
            }else {
                audioController.HitBrick();
            }
            speed += speedIncrease;
            rb.velocity = Vector2.ClampMagnitude(
                new Vector2(rb.velocity.x, rb.velocity.y),1)*speed;
        }

        if (col.collider.name == "Paddle")
        {
            audioController.HitPaddle();
            float newX = BallDirection(
                col.transform.position, transform.position);
            Quaternion angle = Quaternion.AngleAxis(
                newX * col.collider.GetComponent<Paddle>().maxAngleOnBounce, Vector3.back);
            Vector2 newDirection = angle * new Vector2(0, 1);
            speed += speedIncrease;
            rb.velocity = newDirection * speed;
        }
        
        if (col.collider.CompareTag("Void"))
        {
            audioController.DestroyBall();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {
            BallBounce(col.ClosestPoint(transform.position));
        }
        if(col.CompareTag("BreakableBrick"))
        {
            var destroyed = col.gameObject.GetComponent<Brick>().HitBrick(damage);
            if (destroyed)
            {
                audioController.BreakBrick();
            }else {
                audioController.HitBrick();
            }
        }

        if (col.name == "Paddle")
        {
            audioController.HitPaddle();
            float newX = BallDirection(col.transform.position, transform.position);
            Quaternion angle = Quaternion.AngleAxis(newX * col.GetComponent<Paddle>().maxAngleOnBounce, Vector3.back);
            Vector2 newDirection = angle * new Vector2(0, 1);
            speed += speedIncrease;
            rb.velocity = newDirection * speed;
        }
        
        if (col.CompareTag("Void"))
        {
            audioController.DestroyBall();
        }
    }
}