using _Scripts.Save_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenuBehaviour : MonoBehaviour
{
    [SerializeField] ConfirmBehaviour m_ConfirmationWidget;
    [SerializeField] GameObject m_OptionsPanel;

    [SerializeField]
    private ScriptablesWorldAudio _scriptablesWorldAudio;
    private ObservableSound _observableSound;
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        m_OptionsPanel.SetActive(false);

        _observableSound = GetComponent<ObservableSound>();
        _audioManager = GameObject.Find("AudioManagerUI").GetComponent<AudioManager>();
    }

    void OnEnable()
    {
        m_OptionsPanel.SetActive(false);
    }

    public void ClosePauseMenu()
    {
        StateMachine.instance.SetState(new FreeViewState());
        Debug.LogWarning("TODO: Resume game");
        PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound2);
        gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        Debug.Log("Saving...");
        PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound3);

        SaveSystem.instance.SaveGameAsync().Wait();
    }

    public void LoadGame()
    {
        // #TODO : Load game
        PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound3);
        Debug.LogWarning("TODO: Load game");
    }

    // we assume settings panel is covering the pause menu completely
    public void ShowSettings()
    {
        if(m_OptionsPanel == null)
        {
            Debug.LogWarning("Settings panel unbound to pause menu panel");
            return;
        }
        PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
        m_OptionsPanel.gameObject.SetActive(true);
    }

    private void _Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Quit()
    {
        if(m_ConfirmationWidget == null)
        {
            Debug.LogWarning("No confirmation panel, skipping user confirmation.");
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound2);
            _Quit();
            return;
        }
        PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound2);
        m_ConfirmationWidget.AskForConfirmation("Do you really want to quit? Unsaved progress will be lost.", _Quit);
    }

    protected void PlaySound(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
    {
        _observableSound.NotifyObserver(_audioScript, _action);
    }
}