using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBGM : MonoBehaviour
{

    private ObservableSound _observableSound;
    [SerializeField]
    private ScriptablesWorldAudio _scriptablesWorldAudio;
    private AudioManager _audioManager;

    private void Awake()
    {
        _observableSound = GetComponent<ObservableSound>();
        _audioManager = GameObject.Find("AudioManagerMusic").GetComponent<AudioManager>();
    }

    void Start()
    {
        PlayRandomMusic();
    }

    protected void PlaySound(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        _observableSound.NotifyObserver(_audioScript, _action);

    }

    private void PlayRandomMusic()
    {
        int idMusic = Random.Range(0, 3);
        switch (idMusic)
        {
            case 0:
                print("Music 1");
                PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
                break;
            case 1:
                print("Music 2");
                PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound2);
                break;
            case 2:
                print("Music 3");
                PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound3);
                break;
            default:
                print("Problem, it's default music");
                break;
        }

        StartCoroutine(WaitEndMusic());

    }


    IEnumerator WaitEndMusic()
    {
        print(_audioManager.GetMusicLength());
        yield return new WaitForSeconds(_audioManager.GetMusicLength());
        PlayRandomMusic();
    }


}
