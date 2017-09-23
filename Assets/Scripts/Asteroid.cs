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
	public int ChildrenCountFromSuper = 2;
	public ParticleSystem explosion;

	void Start () {
		direction = HelperFunctions.GetRandomDirection (50f);
		speed = Random.Range (1f, 5f);
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
			for (int i = 0; i < ChildrenCountFromSuper; i++) {
				GenerateNewAsteroid ();
			}
		} else {
			GameManager.instance.AddScore(1);
		}
		Instantiate (explosion, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}

	void GenerateNewAsteroid() {
		Vector2 newPosition = new Vector3 (Random.Range (transform.position.x - 1, transform.position.x + 1), transform.position.y);
		GameObject newAsteroid = Instantiate (gameObject, newPosition, transform.rotation);
		newAsteroid.transform.localScale = Vector3.one;
		Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid> ();
		asteroidComp.SetAsteroidType (AsteroidType.normal);
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.tag == "wall")
			Destroy (gameObject);
		
	}
}
