using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IObserver
{
    [SerializeField]
    private GameObject PlayingObject;
    private float volume;
    [SerializeField]
    private AudioSource audioSource;
    private float audioLength;

    public void OnNotify(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        Debug.Log("OnNotify");

        volume = _audioScript.volume;

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
        // enable observers
        PlayingObject.GetComponent<ObservableSound>().AddObserver(this);
    }

    private void OnDisable()
    {
        // disable observers
        if(PlayingObject != null)
            PlayingObject.GetComponent<ObservableSound>().RemoveObserver(this);
    }

    private void PlayWorldSound(AudioClip _audioClip)
    {
        print("play");
        audioLength = _audioClip.length;
        audioSource.clip = _audioClip;
        audioSource.Play();
    }

    public float GetMusicLength()
    {
        return audioLength;
    }

    public void SetMusicLoop(bool loop)
    {
        audioSource.loop = true;
    }

    public void SetMusicVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
