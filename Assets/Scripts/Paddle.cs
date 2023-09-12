using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Paddle : MonoBehaviour
{
    [Tooltip("The height the paddle rests on")][Range(5.0f, -5.0f)] [SerializeField] private float paddleHeight;
    [Tooltip("The speed of the paddle")][Range(0f,10f)][SerializeField] private float moveSpeed = 6f;
    [Tooltip("If true, disable keyboard movement and use mouse position instead")][SerializeField] private bool UseMouseInstead = false;

    [Tooltip("If true, copy the Clamp Right value and put its inverted value as Clamp Left")]public bool sameClampRightAndLeft = true;
    [Tooltip("How far to the right the paddle should be able to go (a non-negative value)")][SerializeField] private float clampRight;
    [Tooltip("How far to the left the paddle should be able to go (a non-positive value)")][SerializeField] private float clampLeft;

    [Tooltip("The maximum angle the ball can bounce off of the paddle")][Range(0f,90f)]
    public float maxAngleOnBounce = 70f;

    private Camera mainCamera = null;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (UseMouseInstead)
        {
            MovementMouse();
        }else {
            MovementKeyboard();
        }
    }

    private void OnValidate()
    {
        if (clampRight < 0.0f)
        {
            Debug.LogWarning("Clamp Right must have a non-negative value.", this);
            clampRight = 0.0f;
        }
        if (clampLeft > 0.0f)
        {
            Debug.LogWarning("Clamp Left must have a non-positive value.", this);
            clampLeft = -0.0f;
        }
        if (sameClampRightAndLeft)
        {
            clampLeft = -clampRight;
        }
        
        transform.position = new Vector3(transform.position.x, paddleHeight);
    }

    private void MovementKeyboard()
    {
        var moveDir = Input.GetAxisRaw("Horizontal");
        transform.position = new Vector3(
            Math.Clamp(
                transform.position.x +
                moveDir * (moveSpeed * Time.deltaTime),
                clampLeft, clampRight),paddleHeight,0);
    }

    private void MovementMouse()
    {
        var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(Math.Clamp(
            mousePos.x, clampLeft, clampRight), paddleHeight);
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
        var currentYPosition = transform.position.y;
        
        Gizmos.DrawLine(new Vector3(lineStartRight, currentYPosition, 0),
            new Vector3(lineStartRight, currentYPosition, 0f) + angleRight*(new Vector3(0, 1, 0)));
        Gizmos.DrawLine(new Vector3(lineStartLeft, currentYPosition, 0),
            new Vector3(lineStartLeft, currentYPosition, 0f) + angleLeft*(new Vector3(0, 1, 0)));
    }
}