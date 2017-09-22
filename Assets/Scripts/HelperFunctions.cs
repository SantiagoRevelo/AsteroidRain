using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions {

	public static Vector2 GetRandomDirection(float angleLimit) {
		Vector3 result = Quaternion.Euler(0, 0, Random.Range(-angleLimit, angleLimit)) * Vector3.down;
		return result;
	}

}
