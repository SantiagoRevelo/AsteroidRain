using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{

    public static Vector2 GetRandomDirection(float angleLimit)
    {
        Vector3 result = Quaternion.Euler(0, 0, Random.Range(-angleLimit, angleLimit)) * Vector3.down;
        return result;
    }

    public static Vector2 GetRandomScreenPostion(float minX, float maxX, float minY, float maxY)
    {
        return new Vector2(Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(minX, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(maxX, 0)).x),
            Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, minY)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, maxY)).y));
    }
}
