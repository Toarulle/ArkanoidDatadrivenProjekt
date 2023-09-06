using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidZone : MonoBehaviour
{
    [HideInInspector] public GameObject ballDestroyed = null;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ball"))
        {
            ballDestroyed = col.gameObject;
            Destroy(col.gameObject);
        }
    }
}
