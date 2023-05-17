﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] Image _musicFill;
        [SerializeField] Image _soundsFill;
        [SerializeField] float _step;

        float _musicValue;
        float _soundsValue;

        [SerializeField]
        private ScriptablesWorldAudio _scriptablesWorldAudio;
        private ObservableSound _observableSound;
        private AudioManager _audioManager;

        void Awake()
        {
            _musicValue = PlayerPrefs.GetFloat("Music", 100f);
            _soundsValue = PlayerPrefs.GetFloat("Sounds", 100f);

            _observableSound = GetComponent<ObservableSound>();
            _audioManager = GameObject.Find("AudioManagerUI").GetComponent<AudioManager>();

            Refresh();
        }

        public void Quit()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            gameObject.SetActive(false);
        }

        public void MusicUp()
        {
            _musicValue = Mathf.Min(100f, _musicValue + _step);
            Refresh();
        }

        public void MusicDown()
        {
            _musicValue = Mathf.Max(0f, _musicValue - _step);
            Refresh();
        }
        
        public void SoundsUp()
        {
            _soundsValue = Mathf.Min(100f, _soundsValue + _step);
            Refresh();
        }
        
        public void SoundsDown()
        {
            _soundsValue = Mathf.Max(0f, _soundsValue - _step);
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound2);
            Refresh();
        }

        void Refresh()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            PlayerPrefs.SetFloat("Music", _musicValue);
            PlayerPrefs.SetFloat("Sounds", _soundsValue);
            
            _musicFill.fillAmount = _musicValue / 100;
            _soundsFill.fillAmount = _soundsValue / 100;
        }

        protected void PlaySound(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
        {
            _observableSound.NotifyObserver(_audioScript, _action);
        }
    }
}