using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITEST : MonoBehaviour
{

    private ObservableSound _observableSound;
    [SerializeField]
    private ScriptablesWorldAudio _scriptablesWorldAudio;

    // Start is called before the first frame update
    void Start()
    {
        _observableSound = GetComponent<ObservableSound>();
        PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
    }

    protected void PlaySound(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        _observableSound.NotifyObserver(_audioScript, _action);
    }

}
