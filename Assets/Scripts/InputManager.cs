using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
		
    }
	
    // Update is called once per frame
    void Update()
    {
        // If mouseClick or screenTap
        if ((Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            EvaluateTap(GetScreenPosition());
        }
    }

    void EvaluateTap(Vector2 screenCoords)
    {
        Asteroid asteroid = GetTouchedAsteroid(screenCoords);
        if (asteroid != null)
        {
            GameManager.instance.HitAsteroid(asteroid);
        }
    }

    Vector2 GetScreenPosition()
    {
        if (Input.touches.Length > 0)
        {
            return (Vector2)Input.touches[0].position;
        }
        return (Vector2)Input.mousePosition;
    }

    Asteroid GetTouchedAsteroid(Vector2 pos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
        if (hits != null)
        {
            foreach (RaycastHit2D hit in hits)
            {
                Asteroid asteroid = hit.transform.GetComponent<Asteroid>();
                if (asteroid != null)
                {					
                    return asteroid;
                }
            }
        }
        return null;
    }
}
