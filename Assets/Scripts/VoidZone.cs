using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ball"))
        {
            FindObjectOfType<GameBehaviour>().Miss(col.gameObject);
        }
    }
}
