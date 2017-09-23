using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

	public Image tick;

	[SerializeField]
	float totalSeconds;
	[SerializeField]
	float currentSecond;


	void Start () {
		
	}

	public void ResetClock(float total) {
		totalSeconds = total;
		currentSecond = 0;
	}

	public void SetCurrentTime(float current) {
		currentSecond = totalSeconds - current;
		tick.transform.rotation = Quaternion.AngleAxis (360 * currentSecond / totalSeconds, Vector3.forward);
	}
}
