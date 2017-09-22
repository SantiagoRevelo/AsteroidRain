using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AsteroidType {
	normal, 
	super
}

public class Asteroid : MonoBehaviour {
	
	public float speed;
	public AsteroidType type;
	public Vector3 direction;
	public int numChildsFromSuper = 2;

	void Start () {
		direction = HelperFunctions.GetRandomDirection (30f);
		speed = Random.Range (1f, 3f);
		type = AsteroidType.normal;
	}

	void Update () {
		transform.position +=  (direction * speed * Time.deltaTime);
		transform.Rotate(Vector3.forward,  speed * 2);
	}

	public void SetAsteroidType(AsteroidType astType) {
		type = astType;
		if (astType == AsteroidType.super)
			transform.localScale = new Vector3 (2f, 2f, 2f);
	}

	public void Hit() {
		if (type == AsteroidType.super) {
			for (int i = 0; i < numChildsFromSuper; i++) {
				GenerateNewAsteroid ();
			}
		}
		// TODO: adds 1 point;

		Debug.Log ("<color=green> Touched</color>");
		Destroy (gameObject);
	}

	void GenerateNewAsteroid() {
		GameObject newAsteroid = Instantiate (gameObject, transform.position, transform.rotation);
		Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid> ();
		asteroidComp.SetAsteroidType (AsteroidType.normal);
	}
}
