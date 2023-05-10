using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTEST : MonoBehaviour
{

    [SerializeField]
    private ScriptablesRelativeAudio _scriptablesRelativeAudio;
    private AudioSource _audioSource;
    private float _volume;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        CallSound(EnumRelativeSounds.Spawn);
    }

    public void CallSound(EnumRelativeSounds _action)
    {
        _volume = _scriptablesRelativeAudio.volume;

        switch (_action)
        {
            case EnumRelativeSounds.Spawn:
                PlayRelativeSound(_scriptablesRelativeAudio._spawnAudio);
                Debug.Log("Spawn audio");
                break;
            case EnumRelativeSounds.Activate:
                PlayRelativeSound(_scriptablesRelativeAudio._activateAudio);
                Debug.Log("Activate audio");
                break;
            case EnumRelativeSounds.Problem:
                PlayRelativeSound(_scriptablesRelativeAudio._problemAudio);
                Debug.Log("Problem audio");
                break;
            default:
                break;
        }
    }

    protected void PlayRelativeSound(AudioClip _audioClip)
    {
        _audioSource.PlayOneShot(_audioClip, _volume);
        Debug.Log(_audioClip);
    }

}
