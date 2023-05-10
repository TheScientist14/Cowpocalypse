using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioRelative", menuName = "Audio/RelativeSounds", order = 5) ]
public class ScriptablesRelativeAudio : ScriptableObject
{
    [Range(0, 1f)]
    public float volume = 1f;

    public AudioClip _spawnAudio;
    public AudioClip _activateAudio;
    public AudioClip _problemAudio;

}
