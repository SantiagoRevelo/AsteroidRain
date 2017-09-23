using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScreen : MonoBehaviour {

	List<GameObject> currentLivesList = new List<GameObject>();
	public GameObject livePrefab;
	public Transform livesContainer;
	public Animator flashAnimator;

	void Start () {
		GameManager.instance.OnLoseLive += HandleOnChangeLivesCount;
	}

	void HandleOnChangeLivesCount(int count, bool flash = false){
		
		foreach (GameObject go in currentLivesList) {
			Destroy (go);
		}
		currentLivesList.Clear ();
		if (count > 0) {
			for (int i = 0; i < count; i++) {
				GameObject newLive = Instantiate (livePrefab, livesContainer);
				currentLivesList.Add (newLive);
			}

			if (flash) {
				flashAnimator.SetTrigger ("hit");
			}
		} else {
			Debug.Log ("no lives");
		}
	}
}
