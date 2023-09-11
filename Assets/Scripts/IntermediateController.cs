using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateController : MonoBehaviour
{
    private GameBehaviour gb = null;
    // Start is called before the first frame update
    void Start()
    {
        gb = FindObjectOfType<GameBehaviour>();
    }

    public void RestartGame()
    {
        gb.RestartGame();
    }
}
