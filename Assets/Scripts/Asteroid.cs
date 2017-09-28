using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AsteroidType {
	NORMAL, 
	SUPER
}

public class Asteroid : MonoBehaviour {
	
	public float speed;
	public AsteroidType type;
	public Vector3 direction;
	public int ChildrenCountFromSuper = 2;
	public bool isAlive;

	private int hitsCount;
	public int HitsCount {
		get {return hitsCount;}
		set { 
			hitsCount = value;
			isAlive = hitsCount > 0;
		}
	}

	void Start () {
		direction = HelperFunctions.GetRandomDirection (50f);
		speed = Random.Range (1f, 5f);
		HitsCount = 1;
	}

	void Update () {
		transform.position +=  (direction * speed * Time.deltaTime);
		transform.Rotate(Vector3.forward,  speed * 2);
	}

	public void SetAsteroidType(AsteroidType astType) {
		type = astType;
		if (astType == AsteroidType.SUPER)
			transform.localScale = new Vector3 (2f, 2f, 2f);
	}

	public void Hit() {
		AudioMaster.instance.Play (SoundDefinitions.EXPLOSION);
		ParticleManager.instance.playParticle(ParticleType.PARTICLE_DUST, transform.position);
		HitsCount--;
	}

	void GenerateNewAsteroid(Vector3 position) {
		Vector2 newPosition = new Vector3 (Random.Range (position.x - 1, position.x + 1), position.y);
		GameObject newAsteroid = Instantiate (gameObject, newPosition, Quaternion.identity);
		newAsteroid.transform.localScale = Vector3.one;
		Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid> ();
		asteroidComp.SetAsteroidType (AsteroidType.NORMAL);
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.tag == "wall")
			GameManager.instance.DestroyAsteroid (gameObject);

		//Hitting downWall player loses a live;
		if (col.name == "DownWall") {
			GameManager.instance.LoseLive();
			AudioMaster.instance.Play (SoundDefinitions.EXPLOSION);
			ParticleManager.instance.playParticle(ParticleType.PARTICLE_DUST, transform.position);
		}		
		
	}
}
