using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IObserver
{
    [SerializeField]
    private GameObject[] _playingObjects;
    private float _volume;
    [SerializeField]
    private AudioSource _audioSource;
    private float _audioLength;
    private int _playingObjectsLenght;

    public void OnNotify(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        Debug.Log("OnNotify");

        _volume = _audioScript.volume;

        switch (_action)
        {
            case EnumWorldSounds.Sound1:
                PlayWorldSound(_audioScript._sound1);
                Debug.Log("Select audio");
                break;
            case EnumWorldSounds.Sound2:
                PlayWorldSound(_audioScript._sound2);
                float lenght = _audioScript._sound2.length;
                Debug.Log("Swipe audio");
                break;
            case EnumWorldSounds.Sound3:
                PlayWorldSound(_audioScript._sound3);
                Debug.Log("Confirm audio");
                break;
            default:
               break;
        }
    }

    private void OnEnable()
    {
        foreach (GameObject _playingObject in _playingObjects)
        {
            // enable observers
            _playingObject.GetComponent<ObservableSound>().AddObserver(this);
        }
    }

    private void OnDisable()
    {
        foreach (GameObject _playingObject in _playingObjects)
        {
            // disable observers
            if (_playingObject != null)
                _playingObject.GetComponent<ObservableSound>().RemoveObserver(this);
        }
    }

    private void PlayWorldSound(AudioClip _audioClip)
    {
        print("play");
        _audioLength = _audioClip.length;
        _audioSource.clip = _audioClip;
        _audioSource.Play();
    }

    public float GetMusicLength()
    {
        return _audioLength;
    }

    public void SetMusicLoop(bool loop)
    {
        _audioSource.loop = true;
    }

    public void SetMusicVolume(float volume)
    {
        _audioSource.volume = volume;
    }
}
