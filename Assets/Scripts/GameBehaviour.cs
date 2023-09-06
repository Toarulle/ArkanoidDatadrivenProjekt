using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehaviour : MonoBehaviour
{

    [SerializeField] private string gameOverScene;
    [SerializeField] private string gamePlayScene;
    
    [SerializeField] private Paddle paddle = null;
    [SerializeField] private VoidZone voidZone = null;
    [SerializeField] private GameObject ballPrefab = null;
    
    private List<GameObject> balls = new List<GameObject>();
    private GameObject ballOnPaddle = null;


    // Start is called before the first frame update
    void Start()
    {
        var firstBall = Instantiate(ballPrefab, paddle.transform.position+new Vector3(0,0.15f), Quaternion.identity);
        balls.Add(firstBall);
        balls.First().transform.SetParent(paddle.transform);
        ballOnPaddle = balls.First();
    }

    // Update is called once per frame
    void Update()
    {
        if (voidZone.ballDestroyed != null)
        {
            balls.Remove(voidZone.ballDestroyed);
            Debug.Log(balls.Count());
        }
        if (balls.Count < 1)
        {
            Debug.Log("GameOver");
            SceneManager.LoadScene(gameOverScene);
        }
        if (ballOnPaddle)
        {
            SendBallFromPaddle();
        }
    }

    private void SendBallFromPaddle()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            paddle.transform.DetachChildren();
            ballOnPaddle.GetComponent<Ball>().SendBall();
            ballOnPaddle = null;
        }
    }
    
    public void RestartGame()
    {
        if (SceneManager.GetActiveScene().name == gameOverScene)
        {
            SceneManager.LoadScene(gamePlayScene);
        }
    }
}
