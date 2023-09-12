using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Timers;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehaviour : MonoBehaviour
{

    [Tooltip("Name of the Game Over Scene")][SerializeField] private string gameOverScene;
    [Tooltip("Name of the Game Play Scene")][SerializeField] private string gamePlayScene;
    [Tooltip("Name of the Level Cleared Scene")][SerializeField] private string levelClearedScene;

    [Tooltip("The amount of lives the player has before losing")][SerializeField] private int lives = 3;
    [Tooltip("The paddle script from the scene (required)")][SerializeField] private Paddle paddle = null;
    [Tooltip("The ball prefab (required)")][SerializeField] private GameObject ballPrefab = null;

    [Tooltip("How many zeroes should be along with the score. E.g. 8 with 2260 points gives: 00002260 (min: 4)")][SerializeField] private int numberAmountInPointsText = 8;
    private int points = 0;
    private List<GameObject> balls = new List<GameObject>();
    private List<GameObject> bricksInPlay = new List<GameObject>();
    private GameObject ballOnPaddle = null;
    private GameObject pointsObject = null;

    private float powerUpTimer = 0;
    private bool powerUpEnabled = false;
    private bool gameCompleted = false;
    private int originalLives;
    

    [HideInInspector]public static GameBehaviour instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        originalLives = lives;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance != this) return;
        if (SceneManager.GetActiveScene() ==
            SceneManager.GetSceneByName(gamePlayScene))
        {
            InitializeLevel();
            FindAllBricksInPlay();
        }
        InitializePoints();
    }
    
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(gamePlayScene) && gameCompleted)
        {
            gameCompleted = false;
            SceneManager.LoadScene(levelClearedScene);
        }
        if (ballOnPaddle)
        {
            SendBallFromPaddle();
        }
        if (powerUpEnabled)
        {
            ResetBallDamage(powerUpTimer);
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
        points = 0;
        lives = originalLives;
        paddle = FindObjectOfType<Paddle>();
        balls = new List<GameObject>();
        FindObjectOfType<LivesBehaviour>().SetupLives(lives);
        var firstBall = Instantiate(ballPrefab, 
            paddle.transform.position+
            new Vector3(0,ballPrefab.transform.localScale.x*2), Quaternion.identity);
        balls.Add(firstBall);
        balls.First().transform.SetParent(paddle.transform);
        ballOnPaddle = balls.First();
    }

    private void FindAllBricksInPlay()
    {
        bricksInPlay = new List<GameObject>();
        var tempList = FindObjectsOfType<Brick>();
        foreach (Brick brick in tempList)
        {
            if (brick.CompareTag("BreakableBrick"))
            {
                bricksInPlay.Add(brick.gameObject);
            }
        }
    }

    public void RemoveBrickFromPlay(GameObject brick)
    {
        bricksInPlay.Remove(brick);
        if (bricksInPlay.Count == 0)
        {
            gameCompleted = true;
        }
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
        var firstBall = Instantiate(ballPrefab,
            paddle.transform.position+
            new Vector3(0,ballPrefab.transform.localScale.x*2), Quaternion.identity);
        balls.Add(firstBall);
        balls.First().transform.SetParent(paddle.transform);
        ballOnPaddle = balls.First();
    }
    
    private void SendBallFromPaddle()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            paddle.transform.DetachChildren();
            ballOnPaddle.GetComponent<Ball>().SendBallFromPaddle();
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

    public void PowerUpFireBall(int newDamage, int time)
    {
        foreach (var ball in balls)
        {
            ball.GetComponent<Ball>().EnableFireBall(newDamage);
        }
        powerUpTimer = Time.time+time;
        powerUpEnabled = true;
    }
    public void SpawnExtraBalls(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var ball = Instantiate(ballPrefab, balls[0].transform.position, Quaternion.identity);
            balls.Add(ball);
            ball.GetComponent<Ball>().SendBallFromPowerUp();
        }
    }

    private void ResetBallDamage(float timeToStop)
    {
        if (Time.time > timeToStop)
        {
            powerUpEnabled = false;
            foreach (var ball in balls)
            {
                ball.GetComponent<Ball>().DisableFireBall();
            }
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
