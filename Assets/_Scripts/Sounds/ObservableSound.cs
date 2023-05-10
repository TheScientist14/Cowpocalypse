using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableSound : MonoBehaviour
{
    // all the observers on this subject
    private List<IObserver> _observer = new List<IObserver>();

    // add observer in the collection
    public void AddObserver(IObserver observer)
    {
        _observer.Add(observer);
    }

    // remove observer in the collection
    public void RemoveObserver(IObserver observer)
    {
        _observer.Remove(observer);
    }

    // notify each observer that an event has occure
    public void NotifyObserver(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        _observer.ForEach((_observer) =>
        {
            _observer.OnNotify(_audioScript, _action);   
        });
    }
}
