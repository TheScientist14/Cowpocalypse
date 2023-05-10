using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    // subject use this method to communicate with the observer
    public void OnNotify(ScriptablesWorldAudio audioScript, EnumWorldSounds action);
}
