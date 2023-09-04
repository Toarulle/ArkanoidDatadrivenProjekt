using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Paddle : MonoBehaviour
{
    [Range(-1.0f, -5.0f)] [SerializeField] private float paddleHeight;

    [SerializeField] private float clampLeft;
    [SerializeField] private float clampRight;

    [SerializeField] private Ball ball = null;

    [Range(0f,90f)]
    public float maxAngleOnBounce = 70f;
    
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(Math.Clamp(mousePos.x, clampLeft, clampRight), paddleHeight);
    }

    private void OnDrawGizmosSelected()
    {
        var lineStartRight = transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 3;
        var lineStartLeft = -lineStartRight;

        Quaternion angleRight = Quaternion.AngleAxis(maxAngleOnBounce, Vector3.back);
        Quaternion angleLeft = Quaternion.AngleAxis(-maxAngleOnBounce, Vector3.back);
        
        Gizmos.DrawLine(new Vector3(lineStartRight, transform.position.y, 0),
            new Vector3(lineStartRight, transform.position.y, 0f) + angleRight*(new Vector3(0, 1, 0)));
        Gizmos.DrawLine(new Vector3(lineStartLeft, transform.position.y, 0),
            new Vector3(lineStartLeft, transform.position.y, 0f) + angleLeft*(new Vector3(0, 1, 0)));
    }
}