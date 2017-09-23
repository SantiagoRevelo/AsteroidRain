using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Button))]
public class AudioButton : MonoBehaviour {
	
	public SoundDefinitions soundDefinition;

	Button parent;
	void Awake() {
		GetComponent<Button>().onClick.AddListener(OnClick);	
	}

	void OnClick()
	{
		AudioMaster.instance.Play(soundDefinition);			
	}
}