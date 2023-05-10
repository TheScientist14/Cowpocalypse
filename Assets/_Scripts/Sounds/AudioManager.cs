using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IObserver
{
    [SerializeField]
    private GameObject UITest;
    private float volume;
    [SerializeField]
    private AudioSource audioSource;

    public void OnNotify(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        Debug.Log("OnNotify");

        volume = _audioScript.volume;

        switch (_action)
        {
            case EnumWorldSounds.Select:
                PlayWorldSound(_audioScript._selectAudio);
                Debug.Log("Select audio");
                break;
            case EnumWorldSounds.Swipe:
                PlayWorldSound(_audioScript._swipeAudio);
                Debug.Log("Swipe audio");
                break;
            case EnumWorldSounds.Confirm:
                PlayWorldSound(_audioScript._confirmAudio);
                Debug.Log("Confirm audio");
                break;
            default:
               break;
        }
    }

    private void OnEnable()
    {
        // enable observers
        UITest.GetComponent<ObservableSound>().AddObserver(this);
    }

    private void OnDisable()
    {
        // disable observers
        if(UITest != null)
            UITest.GetComponent<ObservableSound>().RemoveObserver(this);
    }

    private void PlayWorldSound(AudioClip _audioClip)
    {
        audioSource.PlayOneShot(_audioClip, volume);
    }
}
