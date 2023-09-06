using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Paddle : MonoBehaviour
{
    [Range(-1.0f, -5.0f)] [SerializeField] private float paddleHeight;

    public bool sameClampRightAndLeft = true;
    [SerializeField] private float clampRight;
    [SerializeField] private float clampLeft;

    private float maximumClampRight;
    private float maximumClampLeft;
    
    [Range(0f,90f)]
    public float maxAngleOnBounce = 70f;

    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(Math.Clamp(mousePos.x, clampLeft, clampRight), paddleHeight);
    }

    private void OnValidate()
    {
        if (sameClampRightAndLeft)
        {
            clampLeft = -clampRight;
        }

        transform.position = new Vector3(transform.position.x, paddleHeight);
    }

    private void OnDrawGizmosSelected()
    {
        var paddleWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        //Gizmos for showing the maximum width the paddle can go (corresponding to the "clampLeft" and "clampRight" variables
        Gizmos.DrawLine(new Vector3(clampLeft-paddleWidth/2,paddleHeight-0.5f),new Vector3(clampLeft-paddleWidth/2,paddleHeight+0.5f));
        Gizmos.DrawLine(new Vector3(clampRight+paddleWidth/2,paddleHeight-0.5f),new Vector3(clampRight+paddleWidth/2,paddleHeight+0.5f));
        
        
        //Gizmos for showing the maximum angle the ball can bounce when close to the edge of the paddle
        var lineStartRight = transform.position.x + paddleWidth / 3;
        var lineStartLeft = -lineStartRight;

        Quaternion angleRight = Quaternion.AngleAxis(maxAngleOnBounce, Vector3.back);
        Quaternion angleLeft = Quaternion.AngleAxis(-maxAngleOnBounce, Vector3.back);
        
        Gizmos.DrawLine(new Vector3(lineStartRight, transform.position.y, 0),
            new Vector3(lineStartRight, transform.position.y, 0f) + angleRight*(new Vector3(0, 1, 0)));
        Gizmos.DrawLine(new Vector3(lineStartLeft, transform.position.y, 0),
            new Vector3(lineStartLeft, transform.position.y, 0f) + angleLeft*(new Vector3(0, 1, 0)));
    }
}