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
    [SerializeField] private GameObject ballPrefab = null;
    
    private List<GameObject> balls = new List<GameObject>();
    private GameObject ballOnPaddle = null;

    void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(gamePlayScene))
        {
            InitializeLevel();
        }
    }

    void Update()
    {
        if (ballOnPaddle)
        {
            SendBallFromPaddle();
        }
    }

    private void InitializeLevel()
    {
        var firstBall = Instantiate(ballPrefab, paddle.transform.position+new Vector3(0,0.15f), Quaternion.identity);
        balls.Add(firstBall);
        balls.First().transform.SetParent(paddle.transform);
        ballOnPaddle = balls.First();
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

    public void Miss(GameObject ball)
    {
        balls.Remove(ball);
        Destroy(ball);
        if (balls.Count < 1)
        {
            Debug.Log("GameOver");
            SceneManager.LoadScene(gameOverScene);
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
