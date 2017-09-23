using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {

	public GameObject asteroidPrefab;
	float screenOffset = 25f;

	// Use this for initialization
	void Start () {
		Reposition ();
	}
	// Update is called once per frame
	void Update () {
		
	}

	void Reposition() {
		transform.position = HelperFunctions.GetRandomScreenPostion (screenOffset, Screen.width - screenOffset, Screen.height + (4 * screenOffset), Screen.height + (4 * screenOffset));
		//	new Vector2 ( Random.Range ( Camera.main.ScreenToWorldPoint (new Vector2 (screenOffset, 0f)).x, 
		//											      Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, 0f)).x - screenOffset), Camera.main.ScreenToWorldPoint (new Vector2 (0f, Screen.height)).y + screenOffset);
		//transform.position = new Vector2 ( Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, 0f, 0f)).x, 5f);
	}

	public void SpawnAsteroid(AsteroidType type) {
		GameObject newAsteroid = Instantiate (asteroidPrefab, transform.position, Quaternion.identity);
		Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid> ();
		asteroidComp.SetAsteroidType (type);
		this.Reposition ();
	}
}
