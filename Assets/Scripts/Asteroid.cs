using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AsteroidType
{
	NORMAL,
	SUPER
}

public class Asteroid : MonoBehaviour
{
	public int lives = 1;
	public AsteroidType type;
	public float speed;
	public Vector3 direction;
	public int ChildrenCountFromSuper = 2;

	private int id;
	private int parentId;
	private bool isAlive;
	private int hitsCount;

	public int HitsCount
	{
		get { return hitsCount; }
		set
		{ 
			hitsCount = value;
			IsAlive = value > 0;
			if (value == 0)
			{
				if (type == AsteroidType.SUPER)
					GameManager.instance.spawner.SpawnAsteroid(Id);
			}
		}
	}

	public bool IsAlive
	{
		get
		{
			return isAlive;
		}
		set
		{
			isAlive = value;
			gameObject.SetActive(value);
			// if not alive break the relation with parent
			if (!value)
			{
				ParentId = -1;
			}
		}
	}

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public int ParentId
	{
		get
		{
			return parentId;
		}
		set
		{
			parentId = value;
		}
	}

	void Update()
	{
		transform.position += (direction * speed * Time.deltaTime);
		transform.Rotate(Vector3.forward, speed * 2);
	}

	public void SetAsteroidType(AsteroidType astType)
	{
		type = astType;
		if (astType == AsteroidType.SUPER)
			transform.localScale = new Vector3(2f, 2f, 2f);
	}

    public void Spawn(Vector3 position)
    {
		gameObject.SetActive(true);
		HitsCount = lives;
		transform.position = position;
		direction = HelperFunctions.GetRandomDirection(50f);
		speed = Random.Range(1f, 5f);
		
    }

	public void Hit()
	{
		AudioMaster.instance.Play(SoundDefinitions.EXPLOSION);
		ParticleManager.instance.playParticle(ParticleType.PARTICLE_DUST, transform.position);
		HitsCount--;
	}

	void GenerateChildrens(Vector3 position)
	{
		Vector2 newPosition = new Vector3(Random.Range(position.x - 1, position.x + 1), position.y);
		GameObject newAsteroid = Instantiate(gameObject, newPosition, Quaternion.identity);
		newAsteroid.transform.localScale = Vector3.one;
		Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid>();
		asteroidComp.SetAsteroidType(AsteroidType.NORMAL);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
        if (col.tag == "wall")
        {
            IsAlive = false;
            GameManager.instance.spawner.RemoveChildren(Id);
        }

		//Hitting downWall player loses a live;
		if (col.name == "DownWall")
		{
			GameManager.instance.LoseLive();
			AudioMaster.instance.Play(SoundDefinitions.EXPLOSION);
			ParticleManager.instance.playParticle(ParticleType.PARTICLE_DUST, transform.position);
		}		
		
	}
}
