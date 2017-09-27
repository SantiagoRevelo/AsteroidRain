using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {

	public GameObject asteroidPrefab;
	private float screenOffset = 25f;

	void Start () {
		Reposition ();
	}

	void Reposition() {
		transform.position = HelperFunctions.GetRandomScreenPostion (screenOffset * 3, Screen.width - (screenOffset * 3), Screen.height + (4 * screenOffset), Screen.height + (4 * screenOffset));
	}

	public GameObject SpawnAsteroid(AsteroidType type) {
		this.Reposition ();
		GameObject newAsteroid = Instantiate (asteroidPrefab, transform.position, Quaternion.identity);
		Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid> ();
		asteroidComp.SetAsteroidType (type);
		return newAsteroid;
	}
		
}
