using UnityEngine;
using System.Collections;

// Sound definition
[System.Serializable]
public class GameSound
{
	[SerializeField]
	public SoundDefinitions SoundDef;
	
	[SerializeField]
	public float Volume = 1f;
	
	[SerializeField]		
	public AudioClip[] Audios;
	
	private AudioClip mSound;
	public AudioClip TheSound
	{
		get { return mSound; }
		set { mSound = value; }
	}

	public void SetMainClip()
	{
		int i = Random.Range(0, Audios.Length);
		TheSound = Audios[i];	
	}
}