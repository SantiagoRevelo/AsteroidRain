using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum GameState
{
	INITIALIZING,
	MAIN_MENU,
	PLAYING,
	GAME_OVER
}

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	private const float GAMEPLAY_DURATION = 60f;

	public int score;
	public Text scoreText;
	public Text finalScore;
	public float currentTime;
	public GameTimer gameTimer;

	public int lives;
	public bool amIAlive;
	public Action<int> OnUpdatePlayerLives;
	public Action OnFinishgame;

	public BoxCollider2D leftWall;
	public BoxCollider2D rightWall;
	public BoxCollider2D bottomWall;

	public AsteroidSpawner spawner;

	private GameState currentGameState;
	//private List<GameObject> asteroidsAliveList = new List<GameObject>();

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		SetGameState(GameState.MAIN_MENU);
	}

	public void SetGameState(GameState newGameState)
	{
		if (newGameState != currentGameState)
		{
			switch (newGameState)
			{
				case GameState.MAIN_MENU:
					ScreenManager.Instance.ShowScreen(ScreenDefinitions.MAIN_MENU);
					AudioMaster.instance.PlayMusic(SoundDefinitions.THEME_MAINMENU);
					break;
				case GameState.PLAYING:
					ScreenManager.Instance.ShowScreen(ScreenDefinitions.GAMEPLAY, StartGameplay);
					AudioMaster.instance.PlayMusic(SoundDefinitions.THEME_GAMEPLAY);
					break;
				case GameState.GAME_OVER:
					ScreenManager.Instance.ShowScreen(ScreenDefinitions.GAME_OVER);
					AudioMaster.instance.Play(SoundDefinitions.GAME_OVER);
					break;			
			}
			currentGameState = newGameState;
		}
	}

	public void PlayTheGame()
	{
		SetGameState(GameState.PLAYING);
	}

	public void ReplyGame()
	{
		PlayTheGame();
	}

	public void BackToMainMenu()
	{
        SetGameState(GameState.MAIN_MENU);
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	/// <summary>
	/// Starts the gameplay.
	/// </summary>
	void StartGameplay()
	{
		spawner.InitializeAsteroids();
		RelocateWalls();
		score = 0;
		currentTime = 0;
		amIAlive = true;
		scoreText.text = score.ToString("0000");
		gameTimer.ResetClock(GAMEPLAY_DURATION);
		lives = 5;

        if (OnUpdatePlayerLives != null)
        {
            OnUpdatePlayerLives(lives);
        }

		StartCoroutine(SpawnAsteroids());
	}

	/// <summary>
	/// Finishs the gameplay.
	/// </summary>
	void FinishGameplay()
	{
        spawner.CleanAsteroids();
		finalScore.text = score.ToString("0000");

		if (OnFinishgame != null)
			OnFinishgame();

		SetGameState(GameState.GAME_OVER);
	}

	public void AddScore(int points)
	{
		score += points;
		scoreText.text = score.ToString("0000");
	}

	IEnumerator SpawnAsteroids()
	{
		
		while (currentTime < GAMEPLAY_DURATION && amIAlive)
		{
			for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
			{
				spawner.SpawnAsteroid();
			}
			currentTime++;
			gameTimer.SetCurrentTime(currentTime);

			float currentSecond = GAMEPLAY_DURATION - currentTime;
			if (  currentSecond > 0f && currentSecond <= 5f)
			{
				AudioMaster.instance.Play(SoundDefinitions.CLOCK_TICK);
			}

			yield return new WaitForSeconds(1f);
		}

		FinishGameplay();
	}

	public void HitAsteroid(Asteroid asteroid)
	{
		asteroid.Hit();
		AddScore(1);
	}

	public void LoseLive()
	{
		lives--;

		if (OnUpdatePlayerLives != null)
			OnUpdatePlayerLives(lives);
		
		if (lives <= 0)
		{
			amIAlive = false;
			FinishGameplay();
		}
	}

	/// <summary>
	/// Relocates the collision walls
	/// </summary>
	void RelocateWalls()
	{
		rightWall.transform.position = Vector3.zero;
		rightWall.transform.localScale = Vector3.one;
		rightWall.size = new Vector2(1f, Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height * 2f, 0f)).y);
		rightWall.offset = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x + 1f, 0f);

		bottomWall.transform.position = Vector3.zero;
		bottomWall.transform.localScale = Vector3.one;
		bottomWall.size = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x * 2, 1f);
		bottomWall.offset = new Vector2(0f, Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).y);

		leftWall.transform.position = Vector3.zero;
		leftWall.transform.localScale = Vector3.one;
		leftWall.size = new Vector2(1f, Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height * 2f, 0f)).y);
		leftWall.offset = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).x - 1f, 0f); 
	}
}
