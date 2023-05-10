using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioWorld", menuName = "Audio/WorldSounds", order = 5) ]
public class ScriptablesWorldAudio : ScriptableObject
{
    [Range(0, 1f)]
    public float volume = 1f;

    public AudioClip _selectAudio;
    public AudioClip _confirmAudio;
    public AudioClip _swipeAudio;

}
