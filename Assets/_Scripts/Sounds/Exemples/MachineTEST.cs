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
                PlayRelativeSound(_scriptablesRelativeAudio._spawnAudio, false);
                Debug.Log("Spawn audio");
                break;
            case EnumRelativeSounds.Activate:
                PlayRelativeSound(_scriptablesRelativeAudio._activateAudio, false);
                Debug.Log("Activate audio");
                break;
            case EnumRelativeSounds.Problem:
                PlayRelativeSound(_scriptablesRelativeAudio._problemAudio, false);
                Debug.Log("Problem audio");
                break;
            default:
                break;
        }
    }

    protected void PlayRelativeSound(AudioClip _audioClip, bool loop)
    {
        _audioSource.loop = loop;
        if (loop)
        {
            _audioSource.volume = _volume / 2;
        }
        else
        {
            _audioSource.volume = _volume;
        }
        _audioSource.clip = _audioClip;
        _audioSource.Play();

        Debug.Log(_audioClip);
    }

}
