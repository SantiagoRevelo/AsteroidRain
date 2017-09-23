using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public enum SoundDefinitions
{
	NONE,
	THEME_MAINMENU,
	THEME_GAMEPLAY,
	BUTTON,
	TAP,
	CLOCK_TICK,
	GAME_OVER
}

public class AudioMaster : MonoBehaviour {
	class ClipInfo
	{
		public AudioSource Source { get; set; }
		public float OriginalVolume { get; set; }
		public float currentVolume { get; set; }
		public SoundDefinitions Definition { get; set; }
	}

	public List<GameSound> GameSounds;		// GameSounds List
	private List<ClipInfo> mActiveAudio;   	// Playing Sounds
	private Transform mOriginOfTheSounds;   // Parent of all sounds
	private SoundDefinitions mActiveMusic;	// Active Theme

	public static AudioMaster instance = null;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		mOriginOfTheSounds = transform;

		mActiveAudio = new List<ClipInfo>();
		mActiveMusic = SoundDefinitions.NONE;
	}
	
	void Update() {
	}

	// Reproduce un sonido
	public AudioSource Play(SoundDefinitions soundDef)
	{
		//Create an empty game object
		GameObject soundLoc = CreateSoundLocation("Sound_" + soundDef);
		//Create the Audio source
		AudioSource source = soundLoc != null ? soundLoc.AddComponent<AudioSource>() : null;

		if (source != null)
		{
			//Configure the GameSound
			GameSound gs = GetTheSoundClip(soundDef);
			//Sets the main clip
			gs.SetMainClip();

			if (gs.TheSound != null) {
				//Configure the AudioSource
				SetSource(ref source, soundDef, gs.TheSound, gs.Volume);
				if (source != null && source.clip != null)
				{
						//Play it
					source.Play();
					//Drstroy it when stop
					Destroy(soundLoc, gs.TheSound.length);
				}
				//Set the source as active
				if (mActiveAudio != null && GameSounds != null && GameSounds.Count >= (int)soundDef)
				{
					mActiveAudio.Add(new ClipInfo { Source = source, OriginalVolume = gs.Volume, currentVolume = gs.Volume, Definition = soundDef });
				}
			}
			#if UNITY_EDITOR
			else {
				Debug.Log(string.Format("No hay un Clip de audio asignado al GameSound definido como: {0}.\n" +
					"Revisa el listado de definiciones en el prefab '", soundDef));
			}
			#endif
		}
		return source;
	}
		
	/// <summary>
	/// Play the specified soundDef and ignoreIfExist.
	/// </summary>
	/// <param name='soundDef'>
	/// Sound def.
	/// </param>
	/// <param name='ignoreIfExist'>
	/// ignoreIfExist = 'True', add a new soundañade with this definition
	/// ignoreIfExist = 'False', first stop a sound with this definition befor start playing a new one.
	/// This funciton avoid playing many equal sounds at a time.
	/// </param>
	public AudioSource Play(SoundDefinitions soundDef, bool ignoreIfExist)
	{
		if(!ignoreIfExist)
			StopSound(soundDef);		
		
		return Play(soundDef);
	}

	void StopMusic() {
		if (mActiveMusic != SoundDefinitions.NONE) {
			StopSound (mActiveMusic);
			mActiveMusic = SoundDefinitions.NONE;
		}
	}

	// Play loop theme music
	public AudioSource PlayMusic(SoundDefinitions soundDef) 
	{
		// Only 1 music at a time
		StopMusic ();

		if( IsPlayingSoundDefinition(soundDef))
			StopSound( soundDef );
		
		GameObject soundLoc = CreateSoundLocation("Loop_" + soundDef.ToString());
		//Create the source
		AudioSource source = soundLoc.AddComponent<AudioSource>();

		
		GameSound gs = GetTheSoundClip(soundDef);
		gs.SetMainClip();
		SetSource(ref source, soundDef, gs.TheSound , gs.Volume);
		source.loop = true;
		source.Play();
		
		//Set the source as active
		mActiveAudio.Add(new ClipInfo{Source = source, OriginalVolume = gs.Volume, currentVolume = gs.Volume, Definition = soundDef});
		mActiveMusic = soundDef;
		return source;
	}
	
	// Stops anddestroy Souds
	public void StopSound(SoundDefinitions defToStop) 
	{
		GameObject sound = null;
		ClipInfo ciToRemove = null;
		foreach ( ClipInfo ci in mActiveAudio)
		{
			if (ci.Definition == defToStop) {
				sound = ci.Source.gameObject;
				ciToRemove = ci;
			}
		}

		if (ciToRemove != null)
			mActiveAudio.Remove (ciToRemove);
		
		if( sound != null)	
			Destroy(sound);
	}
	
	// Para y elimina todos los FX y Musicas que haya en la lista de sonidos activos
	public void StopAll() 
	{
		try { 
			foreach (ClipInfo ci in mActiveAudio) {
				if(ci.Source != null)
					Destroy(ci.Source.gameObject);
			}
		}
		catch (NullReferenceException e) {
			Debug.LogErrorFormat (e.Message, e.Data);
		} 
	}
	
	// Crea un Objeto vacío y lo posiciona en la escena y establece su padre en la Jerarquía
	private GameObject CreateSoundLocation(string name)
	{
		//Create an empty game object
		GameObject soundLoc = new GameObject(name);
		if(mOriginOfTheSounds.position != Vector3.zero)
			soundLoc.transform.position = mOriginOfTheSounds.position;
		else
			soundLoc.transform.position = transform.position;
		
		soundLoc.transform.parent = mOriginOfTheSounds;
		return soundLoc;
	}		

	
	//Busca y retorna un GameSound y establece cual es el clip a reproducir
	//(Atencion: En principio no puede usar una misma definición para varios sonidos. El algoritmo devuelve el primero que encuentre)
	GameSound GetTheSoundClip(SoundDefinitions soundDef)
	{
		// Seleccionamos el clip definido como el parametro soundDef 
		GameSound gs = (from g in GameSounds
						where g.SoundDef == soundDef
				select g).FirstOrDefault();
		return gs;		
	}
	
	// Establece los parametros del AudioSource
	private void SetSource(ref AudioSource source, SoundDefinitions soundDef, AudioClip clip, float volume) 
	{
		source.rolloffMode = AudioRolloffMode.Logarithmic;
		source.dopplerLevel = 0.2f;
		source.minDistance = 150;
		source.maxDistance = 1500;
		source.clip = clip;
		source.volume = volume;
		source.pitch = 1;
	}
	
	private bool IsPlayingSoundDefinition(SoundDefinitions soundDef)
	{
		bool isPlaying = false;
		foreach(ClipInfo clip in mActiveAudio)
		{
			if(clip.Definition == soundDef)
			{
				isPlaying = true;
			}
		}
		return isPlaying;
	}
	/*
	// Actualiza los AudioSources activos, y los que ya no se reproduzcan los elimina de la lista
	private void UpdateActiveAudio() 
	{
		if (mToRemove == null) 
			return;

		foreach (var audioClip in mActiveAudio) 
		{
			if (!audioClip.Source) 
			{
				mToRemove.Add(audioClip);
			}
		}

		//cleanup
		foreach (var audioClip in mToRemove) {
			mActiveAudio.Remove(audioClip); 
		}
	}
	*/
}
