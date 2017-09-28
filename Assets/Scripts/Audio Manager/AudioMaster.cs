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
    EXPLOSION,
    CLOCK_TICK,
    GAME_OVER
}

public class AudioMaster : MonoBehaviour
{
	
    public static AudioMaster instance = null;

    /// <summary>
    /// Clip info.
    /// Contains the data of a Playing sound.
    /// </summary>
    class ClipInfo
    {
        public AudioSource Source { get; set; }

        public float OriginalVolume { get; set; }

        public float currentVolume { get; set; }

        public SoundDefinitions Definition { get; set; }
    }

    // GameSounds List
    public List<GameSound> gameSoundList;
    // Playing Sounds
    private List<ClipInfo> activeAudio;
    // Parent of all sounds
    private Transform theSoundsParent;
    // Active Music Theme
    private SoundDefinitions activeMusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        theSoundsParent = transform;

        activeAudio = new List<ClipInfo>();
        activeMusic = SoundDefinitions.NONE;
    }

    /// <summary>
    /// Play the specified soundDefinition.
    /// </summary>
    /// <param name="soundDefinition">Sound definition.</param>
    public AudioSource Play(SoundDefinitions soundDefinition)
    {
        //Create an empty game object
        GameObject soundObj = CreateSoundObject(soundDefinition + "_sfx");
        //Create the Audio source
        AudioSource source = soundObj != null ? soundObj.AddComponent<AudioSource>() : null;

        if (source != null)
        {
            //Configure the GameSound
            GameSound gs = GetTheSoundClip(soundDefinition);
            //Sets the main clip
            gs.SetMainClip();

            if (gs.TheSound != null)
            {
                //Configure the AudioSource
                SetSourceSettings(ref source, soundDefinition, gs.TheSound, gs.Volume);
                if (source != null && source.clip != null)
                {
                    //Play it
                    source.Play();
                    //Drstroy it when stop
                    Destroy(soundObj, gs.TheSound.length);
                }
                //Set the source as active
                if (activeAudio != null && gameSoundList != null && gameSoundList.Count >= (int)soundDefinition)
                {
                    activeAudio.Add(new ClipInfo { Source = source, OriginalVolume = gs.Volume, currentVolume = gs.Volume, Definition = soundDefinition });
                }
            }
			#if UNITY_EDITOR
			else
            {
                Debug.Log(string.Format("The GameSound {0}.\n has not set any audio clip.", soundDefinition));
            }
            #endif
        }
        return source;
    }

    /// <summary>
    /// Play a loop music.
    /// </summary>
    /// <returns>The music.</returns>
    /// <param name="soundDef">Sound def.</param>
    public AudioSource PlayMusic(SoundDefinitions soundDef)
    {
        // Only 1 music at a time
        StopMusic();

        if (IsPlayingSoundDefinition(soundDef))
            StopSound(soundDef);
		
        GameObject soundObj = CreateSoundObject(soundDef.ToString() + "_music");

        //Create the audio source component
        AudioSource source = soundObj.AddComponent<AudioSource>();

		
        GameSound gs = GetTheSoundClip(soundDef);
        gs.SetMainClip();
        SetSourceSettings(ref source, soundDef, gs.TheSound, gs.Volume);
        source.loop = true;
        source.Play();
		
        //Set the source as active
        activeAudio.Add(new ClipInfo{ Source = source, OriginalVolume = gs.Volume, currentVolume = gs.Volume, Definition = soundDef });
        activeMusic = soundDef;
        return source;
    }

    /// <summary>
    /// Stops the sound.
    /// </summary>
    /// <param name="defToStop">SoundDefinition to stop.</param>
    public void StopSound(SoundDefinitions defToStop)
    {
        GameObject sound = null;
        ClipInfo ciToRemove = null;
        foreach (ClipInfo ci in activeAudio)
        {
            if (ci.Definition == defToStop)
            {
                sound = ci.Source.gameObject;
                ciToRemove = ci;
            }
        }

        if (ciToRemove != null)
            activeAudio.Remove(ciToRemove);
		
        if (sound != null)
            Destroy(sound);
    }

    /// <summary>
    /// Stops the current playing loop music.
    /// </summary>
    void StopMusic()
    {
        if (activeMusic != SoundDefinitions.NONE)
        {
            StopSound(activeMusic);
            activeMusic = SoundDefinitions.NONE;
        }
    }

    /// <summary>
    /// Stops all sfx and music loops.
    /// </summary>
    public void StopAllPlayingSounds()
    {
        try
        { 
            foreach (ClipInfo ci in activeAudio)
            {
                if (ci.Source != null)
                    Destroy(ci.Source.gameObject);
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogErrorFormat(e.Message, e.Data);
        } 
    }

    /// <summary>
    /// Creates the sound object.
    /// </summary>
    /// <returns>The sound object.</returns>
    /// <param name="name">Name.</param>
    private GameObject CreateSoundObject(string name)
    {
        //Create an empty game object
        GameObject soundLoc = new GameObject(name);
        if (theSoundsParent.position != Vector3.zero)
            soundLoc.transform.position = theSoundsParent.position;
        else
            soundLoc.transform.position = transform.position;
		
        soundLoc.transform.parent = theSoundsParent;
        return soundLoc;
    }

    /// <summary>
    /// Gets the sound clip.
    /// </summary>
    /// <returns>The the sound clip. Attention: Can't use more than once same sound definition because only the first defined will be return;</returns>
    /// <param name="soundDef">Sound definition</param>
    GameSound GetTheSoundClip(SoundDefinitions soundDef)
    {
        GameSound gs = (from g in gameSoundList
                              where g.SoundDef == soundDef
                              select g).FirstOrDefault();
        return gs;		
    }

    /// <summary>
    /// Sets the source.
    /// </summary>
    /// <param name="source">Source.</param>
    /// <param name="soundDef">Sound Definition</param>
    /// <param name="clip">Clip</param>
    /// <param name="volume">Volume</param>
    private void SetSourceSettings(ref AudioSource source, SoundDefinitions soundDef, AudioClip clip, float volume)
    {
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0.2f;
        source.minDistance = 150;
        source.maxDistance = 1500;
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1;
    }

    /// <summary>
    /// Determines whether this instance is playing a sound definition
    /// </summary>
    /// <returns><c>true</c> if this instance is playing the specified SoundDefinitions; otherwise, <c>false</c>.</returns>
    /// <param name="soundDef">Sound def.</param>
    private bool IsPlayingSoundDefinition(SoundDefinitions soundDef)
    {
        bool isPlaying = false;
        foreach (ClipInfo clip in activeAudio)
        {
            if (clip.Definition == soundDef)
            {
                isPlaying = true;
            }
        }
        return isPlaying;
    }
}
