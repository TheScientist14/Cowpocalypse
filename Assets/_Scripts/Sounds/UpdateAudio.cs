using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateAudio : MonoBehaviour
{
    private float worldSoundVolume;
    private float relativeSoundVolume;
    private AudioManager[] _audioManagers;


    // call this to change audios in the map
    public void UpdateAllAudio()
    {
        // get player's prefs info
        worldSoundVolume = PlayerPrefs.GetFloat("Music", 100f);
        relativeSoundVolume = PlayerPrefs.GetFloat("Sounds", 100f);

        _audioManagers = GameObject.FindObjectsOfType<AudioManager>();

        // Debug.Log("Player prefs: " + worldSoundVolume);

        // get all audios and set parameters
        foreach(var worldAudio in ItemCreator.LoadAllResourceAtPath<ScriptablesWorldAudio>("Scriptable objects/Sounds/Worlds"))
        {
            SetWorldAudioVolume(worldAudio);
        }
        foreach(var relativeAudio in ItemCreator.LoadAllResourceAtPath<ScriptablesRelativeAudio>("Scriptable objects/Sounds/Relatives"))
        {
            SetRelativeAudioVolume(relativeAudio);
        }
    }

    // do not call
    private void SetWorldAudioVolume(ScriptablesWorldAudio wSO)
    {
        // change sound for next sound that will be play
        wSO.volume = worldSoundVolume / 100f;

        // apply anytime in the music
        foreach(var audioMan in _audioManagers)
        {
            audioMan.SetMusicVolume(worldSoundVolume / 100f);
        }

        // Debug.Log("Volume du scriptable: " + wSO.volume);
    }

    private void SetRelativeAudioVolume(ScriptablesRelativeAudio rSO)
    {
        rSO.volume = relativeSoundVolume / 100f;
    }
}
