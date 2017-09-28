using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    public GameObject asteroidPrefab;
    private float screenOffset = 25f;

    private List<AsteroidType> asteroidsPoolPattern = new List<AsteroidType>()
        {
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.NORMAL,
            AsteroidType.SUPER,
            AsteroidType.SUPER,
        };

    private List<Asteroid> asteroidsPool;

    void Start()
    {
        Reposition();
    }

    void Reposition()
    {
        transform.position = HelperFunctions.GetRandomScreenPostion(screenOffset * 3, Screen.width - (screenOffset * 3), Screen.height + (4 * screenOffset), Screen.height + (4 * screenOffset));
    }

    public void InitializeAsteroids() {
        if (asteroidsPool == null)
        {
            asteroidsPool = new List<Asteroid>();
        }
        else
        {
            CleanAsteroids();
        }

        for (int i = 0; i < asteroidsPoolPattern.Count; i++) 
        {
            asteroidsPool.Add(GenerateAsteroid(asteroidsPoolPattern[i], i));
        }
    }

    public void CleanAsteroids()
    {
        asteroidsPool.ForEach(a => Destroy(a.gameObject));
        asteroidsPool.Clear();
    }


    Asteroid GenerateAsteroid(AsteroidType type, int id) {
        GameObject newAsteroid = Instantiate(asteroidPrefab, transform.position, Quaternion.identity);
        Asteroid asteroidComp = newAsteroid.GetComponent<Asteroid>();
        asteroidComp.SetAsteroidType(type);
        asteroidComp.Id = id;
        asteroidComp.ParentId = -1;
        newAsteroid.SetActive(false);
        return asteroidComp;
    }

    public void SpawnAsteroid(int parentID = -1)
    {
        //if has parentID they are children and we must spwan them
        if (parentID != -1)
        {
            Asteroid parentAsteroid = asteroidsPool.Find(p => p.Id == parentID); 
            List<Asteroid> children = asteroidsPool.FindAll(a => !a.IsAlive && a.type == AsteroidType.NORMAL && a.ParentId == parentID);
            children.ForEach(ch => ch.Spawn(parentAsteroid.transform.position));
            Debug.LogFormat("<color=blue> Asteroid [{0}] created {1} childrens</color>", parentAsteroid.Id, children.Count);
        }
        else
        {
            this.Reposition();
            Asteroid availableAsteroid = GetAvailableAsteroid(parentID);
            if (availableAsteroid)
            {
                availableAsteroid.Spawn(transform.position);
            }
        }
    }

    Asteroid GetAvailableAsteroid(int parentID)
    {
        bool canGenerateSuperAsteroid = (Random.Range(0, 10) < 3) && asteroidsPool.FindAll(a => a.IsAlive && a.type == AsteroidType.NORMAL).Count >= 2;
        Asteroid nextAsteroid;

        if (canGenerateSuperAsteroid)
        {
            // A super asteroid has N children that we need to reserve
            nextAsteroid = asteroidsPool.Find(a => !a.IsAlive && a.type == AsteroidType.SUPER);
            if (nextAsteroid)
            {
                List<Asteroid> children = asteroidsPool.FindAll(a => !a.IsAlive && a.type == AsteroidType.NORMAL && a.ParentId == -1);
                if (children.Count >= nextAsteroid.ChildrenCountFromSuper)
                {
                    children = children.GetRange(0, nextAsteroid.ChildrenCountFromSuper);
                    children.ForEach(ch => ch.ParentId = nextAsteroid.Id);
                }
                else
                {
                    // if not children availables then no superAsteroid
                    nextAsteroid = null;
                }
            }
        }
        else
        {
            nextAsteroid = asteroidsPool.Find(a => !a.IsAlive && a.ParentId == parentID && a.type == AsteroidType.NORMAL);
        }

        return nextAsteroid;
    }

    public void RemoveChildren(int parentId) {
        asteroidsPool.FindAll(a => a.ParentId == parentId).ForEach(child => child.ParentId = -1);
    }
}
