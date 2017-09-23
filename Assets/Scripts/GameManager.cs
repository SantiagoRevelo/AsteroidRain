using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState {
	Initializing,
	MainMenu,
	Playing,
	GameOver
}

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	float GAMEPLAY_DURATION = 60f;
	public int score;
	public Text scoreText;
	public Text finalScore;
	public int currentTime;
	public GameTimer gameTimer;

	public BoxCollider2D leftWall;
	public BoxCollider2D rightWall;
	public BoxCollider2D bottomWall;

	public AsteroidSpawner spawner;

	GameState currentGameState;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start () {
		SetGameState(GameState.MainMenu);
	}

	public void SetGameState(GameState newGameState) {
		if (newGameState != currentGameState) {
			switch (newGameState) {
			case GameState.MainMenu:
				ScreenManager.Instance.ShowScreen (ScreenDefinitions.MAIN_MENU);
				break;
			case GameState.Playing:
				ScreenManager.Instance.ShowScreen (ScreenDefinitions.GAMEPLAY, StartGameplay);
				break;
			case GameState.GameOver:
				ScreenManager.Instance.ShowScreen (ScreenDefinitions.GAME_OVER);
				break;			
			}
			currentGameState = newGameState;
		}
	}

	public void PlayTheGame(){
		SetGameState (GameState.Playing);
	}

	public void ReplyGame() {
		PlayTheGame ();
	}

	public void BackToMainMenu() {
		SetGameState (GameState.MainMenu);
	}

	public void ExitGame() {
		Application.Quit ();
	}

	/// <summary>
	/// Starts the gameplay.
	/// </summary>
	void StartGameplay() {
		RelocateWalls ();
		score = 0;
		currentTime = 0;
		scoreText.text = score.ToString("0000");
		gameTimer.ResetClock (GAMEPLAY_DURATION);
		StartCoroutine (SpawnAsteroids());
	}

	/// <summary>
	/// Finishs the gameplay.
	/// </summary>
	void FinishGameplay() {
		finalScore.text = score.ToString ("0000");
		GameObject[] asteroidsAlive = GameObject.FindGameObjectsWithTag ("asteroid");
		foreach (GameObject asteroid in asteroidsAlive)
			Destroy (asteroid);

		SetGameState(GameState.GameOver);
	}

	public void AddScore(int points) {
		score += points;
		scoreText.text = score.ToString("0000");
	}


	IEnumerator SpawnAsteroids() {
		bool generateSuperAsteroid;
		while (currentTime < GAMEPLAY_DURATION) {
			//TODO: can spawn ?? -> Define spawn rules;
			for (int i = 0; i < Random.Range (1, 5); i++) {
				generateSuperAsteroid = Random.Range (0, 50) <= 1;
				spawner.SpawnAsteroid (generateSuperAsteroid ? AsteroidType.super : AsteroidType.normal);
			}
			currentTime++;
			gameTimer.SetCurrentTime (currentTime);

			yield return new WaitForSeconds (1f);
		}
		FinishGameplay ();
	}


	/// <summary>
	/// Relocates the collision walls that destroy miss asteroids.
	/// </summary>
	void RelocateWalls() {
		rightWall.transform.position = Vector3.zero;
		rightWall.transform.localScale = Vector3.one;
		rightWall.size = new Vector2 (1f, Camera.main.ScreenToWorldPoint (new Vector3 (0f, Screen.height * 2f, 0f)).y);
		rightWall.offset = new Vector2 (Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, 0f, 0f)).x + 4f, 0f);

		bottomWall.transform.position = Vector3.zero;
		bottomWall.transform.localScale = Vector3.one;
		bottomWall.size = new Vector2 (Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width * 2f, 0f, 0f)).x, 1f);
		bottomWall.offset = new Vector2 (0f, Camera.main.ScreenToWorldPoint (new Vector3 (0f, 0f, 0f)).y - 4f);

		leftWall.transform.position = Vector3.zero;
		leftWall.transform.localScale = Vector3.one;
		leftWall.size = new Vector2 (1f, Camera.main.ScreenToWorldPoint (new Vector3 (0f, Screen.height * 2f, 0f)).y);
		leftWall.offset = new Vector2 (Camera.main.ScreenToWorldPoint (new Vector3 (0f, 0f, 0f)).x - 4f, 0f); 
	}
}
