using System;
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
        
        void Awake()
        {
            _musicValue = PlayerPrefs.GetFloat("Music", 100f);
            _soundsValue = PlayerPrefs.GetFloat("Sounds", 100f);

            Refresh();
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
            Refresh();
        }

        void Refresh()
        {
            PlayerPrefs.SetFloat("Music", _musicValue);
            PlayerPrefs.SetFloat("Sounds", _soundsValue);
            
            _musicFill.fillAmount = _musicValue / 100;
            _soundsFill.fillAmount = _soundsValue / 100;
        }
    }
}