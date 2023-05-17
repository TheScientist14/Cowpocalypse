using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioWorld", menuName = "Audio/WorldSounds", order = 5) ]
public class ScriptablesWorldAudio : ScriptableObject
{
    [Range(0, 1f)]
    public float volume = 1f;

    public AudioClip _sound1;
    public AudioClip _sound2;
    public AudioClip _sound3;

}
