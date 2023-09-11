using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehaviour : MonoBehaviour
{

    [Tooltip("Name of the Game Over Scene")][SerializeField] private string gameOverScene;
    [SerializeField] private string gamePlayScene;
    [SerializeField] private string levelClearedScene;

    [Tooltip("The amount of lives the player has before losing")][SerializeField] private int lives = 3;
    [Tooltip("The paddle Prefab (required)")][SerializeField] private Paddle paddle = null;
    [Tooltip("The ball prefab (required)")][SerializeField] private GameObject ballPrefab = null;

    [Tooltip("How many zeroes should be along with the score. E.g. 8 with 2260 points gives: 00002260 (min: 4)")][SerializeField] private int numberAmountInPointsText = 8;
    private int points = 0;
    private List<GameObject> balls = new List<GameObject>();
    private GameObject ballOnPaddle = null;
    private GameObject pointsObject = null;

    public static GameBehaviour instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(gamePlayScene))
        {
            InitializeLevel();
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(levelClearedScene) || SceneManager.GetActiveScene() == SceneManager.GetSceneByName(gameOverScene))
        {
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializePoints();
    }
    
    void Update()
    {
        if (ballOnPaddle)
        {
            SendBallFromPaddle();
        }
    }

    private void OnValidate()
    {
        if (numberAmountInPointsText < 4)
        {
            Debug.LogWarning("The least amount of numbers is 4",this);
            numberAmountInPointsText = 4;
        }
        if (paddle == null)
        {
            Debug.LogError("Paddle can't be null",this);
        }
        if (ballPrefab == null)
        {
            Debug.LogError("Ball Prefab can't be null",this);
        }
    }

    private void InitializeLevel()
    {
        FindObjectOfType<LivesBehaviour>().SetupLives(lives);
        var firstBall = Instantiate(ballPrefab, paddle.transform.position+new Vector3(0,ballPrefab.transform.localScale.x*2), Quaternion.identity);
        balls.Add(firstBall);
        balls.First().transform.SetParent(paddle.transform);
        ballOnPaddle = balls.First();
    }

    private void InitializePoints()
    {
        pointsObject = GameObject.Find("Points");
        UpdatePointsText(points);
    }

    public void AddPoints(int brickPoints)
    {
        points += brickPoints;
        UpdatePointsText(points);
    }

    private void UpdatePointsText(int pts)
    {
        pointsObject.GetComponent<TextMeshProUGUI>().SetText(CreatePointsText(pts));
    }
    
    private void ResetBall()
    {
        var firstBall = Instantiate(ballPrefab, paddle.transform.position+new Vector3(0,ballPrefab.transform.localScale.x*2), Quaternion.identity);
        balls.Add(firstBall);
        balls.First().transform.SetParent(paddle.transform);
        ballOnPaddle = balls.First();
    }
    
    private void SendBallFromPaddle()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
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
        if (balls.Count >= 1) return;
        LoseLife();
        ResetBall();
    }

    private void LoseLife()
    {
        lives--;
        FindObjectOfType<LivesBehaviour>().RemoveOneLifeFromUI(lives);
        if (lives < 1)
        {
            SceneManager.LoadScene(gameOverScene);
            return;
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(gamePlayScene);
    }

    private string CreatePointsText(int points)
    {
        string pointsString = points.ToString();
        while (pointsString.Length < numberAmountInPointsText)
        {
            pointsString = pointsString.Insert(0, "0");
        }

        return pointsString;
    }
}
