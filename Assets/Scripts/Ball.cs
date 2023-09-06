using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class Ball : MonoBehaviour
{
    [Range(1.0f, 5.0f)] [SerializeField] private float speed;

    public int damage = 1;

    private Vector2 direction;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.velocity.Set(0,0);
    }

    public void SendBall()
    {
        float randomDirection = Random.Range(-1f, 1f);
        direction = Vector2.ClampMagnitude(new Vector2(randomDirection, 1), 1);
        rb.velocity = direction * speed;
    }
    
    private float BallDirection(Vector2 racketPosition, Vector2 ballPosition, float racketWidth)
    {
        return Math.Clamp(ballPosition.x - racketPosition.x, -1f, 1f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (transform.parent != null) return;
        
        if(col.collider.CompareTag("BreakableBrick"))
        {
            col.gameObject.GetComponent<Brick>().HitBrick(damage);
        }

        if (col.collider.name == "Paddle")
        {
            float newX = BallDirection(col.transform.position, transform.position,
                col.collider.bounds.size.x);

            //Vector2 newDirection = new Vector2(newX, 1).normalized;
            Quaternion angle = Quaternion.AngleAxis(newX * col.collider.GetComponent<Paddle>().maxAngleOnBounce, Vector3.back);
            Vector2 newDirection = angle * new Vector2(0, 1);
            col.otherRigidbody.velocity = newDirection * speed;
        }

        if (col.collider.CompareTag("Void"))
        {
            Destroy(gameObject);
        }
    }
}