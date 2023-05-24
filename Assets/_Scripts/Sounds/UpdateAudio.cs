using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateAudio : MonoBehaviour
{
    private float worldSoundVolume;
    private float relativeSoundVolume;

    [SerializeField] private ScriptablesWorldAudio[] worldAudios;
    [SerializeField] private ScriptablesRelativeAudio[] relativeAudios;

    // Start is called before the first frame update
    void Start()
    {
        worldSoundVolume = PlayerPrefs.GetFloat("Music", 100f);
        relativeSoundVolume = PlayerPrefs.GetFloat("Sounds", 100f);
    }

    // call this to change audios in the map
    public void GetAllAudio()
    {
        worldAudios = GameObject.FindObjectsOfType<ScriptablesWorldAudio>();
        relativeAudios = GameObject.FindObjectsOfType<ScriptablesRelativeAudio>();

        SetAudioVolume();
    }

    // do not call
    private void SetAudioVolume()
    {
        foreach (var worldAudio in worldAudios)
        {
            worldAudio.volume = worldSoundVolume;
        }
        foreach (var relativeAudio in relativeAudios)
        {
            relativeAudio.volume = relativeSoundVolume;
        }
    }
}
