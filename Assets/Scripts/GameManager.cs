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

	public float GAMEPLAY_DURATION = 60f;
	public int score;
	public Text scoreText;
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
		BeginGame ();
	}

	void Update () {
	}


	void SetGameState(GameState newGameState) {
		switch (newGameState) {
		case GameState.MainMenu:
			break;
		case GameState.Playing:
			break;
		case GameState.GameOver:
			break;			
		}
	}



	void BeginGame() {
		RelocateWalls ();
		score = 0;
		scoreText.text = score.ToString("0000");
		gameTimer.ResetClock (GAMEPLAY_DURATION);
		StartCoroutine (SpawnAsteroids());
	}

	void FinishGame() {
		GameObject[] asteroidsAlive = GameObject.FindGameObjectsWithTag ("asteroid");
		foreach (GameObject asteroid in asteroidsAlive)
			Destroy (asteroid);
	}

	public void AddScore(int points) {
		score += points;
		scoreText.text = score.ToString("0000");
	}

	IEnumerator SpawnAsteroids() {
		bool generateSuperAsteroid;
		while (currentTime < GAMEPLAY_DURATION) {
			//TODO: can spawn ?? -> Define spawn rules;
			generateSuperAsteroid = Random.Range (0, 10) <= 1;
			spawner.SpawnAsteroid (generateSuperAsteroid ? AsteroidType.super : AsteroidType.normal);
			currentTime++;
			gameTimer.SetCurrentTime (currentTime);

			yield return new WaitForSeconds (1f);
		}
		FinishGame ();
	}

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
