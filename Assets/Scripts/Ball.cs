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
    [Tooltip("How much the ball speed increases with each bounce (not walls)")] [SerializeField] private float speedIncrease = 0.1f;

    [Tooltip("How much damage the ball does when it hits a brick (A non-zero, non-negative value")] public int damage = 1;

    [SerializeField] private List<AudioClip> hitWallAudioList;
    [SerializeField] private List<AudioClip> hitBrickAudioList;
    [SerializeField] private AudioClip destroyBallAudio;
    [SerializeField] private AudioClip sendBallFromPaddleAudio;

    private AudioSource audioSource = null;
    private Vector2 direction;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        rb.velocity.Set(0,0);
    }

    private void OnValidate()
    {
        if (speed <= 0.0f)
        {
            Debug.LogWarning("Speed must have a non-zero, non-negative value.", this);
            speed = 0.1f;
        }
        if (damage <= 0)
        {
            Debug.LogWarning("Damage must have a non-zero, non-negative value.", this);
            damage = 1;
        }
    }

    public void SendBall()
    {
        float randomDirection = Random.Range(-1f, 1f);
        direction = Vector2.ClampMagnitude(new Vector2(randomDirection, 1), 1);
        rb.velocity = direction * speed;
    }
    
    private float BallDirection(Vector2 racketPosition, Vector2 ballPosition)
    {
        return Math.Clamp(ballPosition.x - racketPosition.x, -1f, 1f);
    }

    private void BallBounce(Vector2 contactPoint)
    {
        var currentVelocity = rb.velocity;
        var newXVelocity = currentVelocity.x;
        var newYVelocity = currentVelocity.y;
        var position = transform.position;
        Debug.Log("X: " + newXVelocity + " - Y: " + newYVelocity);
        if (contactPoint.x - position.x != 0)
        {
            newXVelocity = currentVelocity.x *-1;
        }

        if (contactPoint.y - position.y !=0)
        {
            newYVelocity = currentVelocity.y *-1;
        }
        Debug.Log("X: " + newXVelocity + " - Y: " + newYVelocity);
        rb.velocity = new Vector2(newXVelocity, newYVelocity);
        Debug.Log("X: " + rb.velocity.x + " - Y: " + rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (transform.parent != null) return;
        
        //BallBounce(col.GetContact(0).point);
        
        if(col.collider.CompareTag("BreakableBrick"))
        {
            audioSource.clip = hitBrickAudioList[Random.Range(0,hitBrickAudioList.Count)];
            audioSource.Play();
            col.gameObject.GetComponent<Brick>().HitBrick(damage);
            speed += speedIncrease;
        }

        if (col.collider.name == "Paddle")
        {
            audioSource.clip = hitWallAudioList[Random.Range(0,hitWallAudioList.Count)];
            audioSource.Play();
            float newX = BallDirection(col.transform.position, transform.position);

            Quaternion angle = Quaternion.AngleAxis(newX * col.collider.GetComponent<Paddle>().maxAngleOnBounce, Vector3.back);
            Vector2 newDirection = angle * new Vector2(0, 1);
            speed += speedIncrease;
            col.otherRigidbody.velocity = newDirection * speed;
        }

        if (col.collider.CompareTag("Wall"))
        {
            audioSource.clip = hitWallAudioList[Random.Range(0,hitWallAudioList.Count)];
            audioSource.Play();
        }
        if (col.collider.CompareTag("Void"))
        {
            audioSource.clip = destroyBallAudio;
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}