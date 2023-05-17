using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Camera _camera;
        [SerializeField] Transform _titleTransform;
        [SerializeField] RectTransform _buttonsRect;
        [SerializeField] CanvasGroup _buttons;
        [SerializeField] GameObject _optionPanel;
        [SerializeField] GameObject _WarningPanel;

        private ObservableSound _observableSound;
        [SerializeField]
        private ScriptablesWorldAudio _scriptablesWorldAudio;
        private AudioManager _audioManager;

        private void Awake()
        {
            _observableSound = GetComponent<ObservableSound>();
            _audioManager = GameObject.Find("AudioManagerUI").GetComponent<AudioManager>();
        }

        void Start()
        {
            _camera.orthographicSize = 2;
            _titleTransform.position = new Vector2(0, 7);
            _buttons.alpha = 0;
            _buttonsRect.anchoredPosition = new Vector2(-400, 0);
            
            var sequence = DOTween.Sequence();
            sequence.Append(_camera.DOOrthoSize(5, 2));
            sequence.Append(_titleTransform.DOMoveY(3, 0.25f).SetEase(Ease.OutBack));
            sequence.Append(_camera.DOShakePosition(2f));
            sequence.Append(_buttons.DOFade(1, 2));
            sequence.Insert(3, _buttonsRect.DOAnchorPosX(0, 2));
        }

        public void NewGame()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound2);
            SceneManager.LoadScene(1);
        }

        public void Play()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            _WarningPanel.SetActive(true);
        }

        public void Load()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            Debug.Log("Load !");
        }

        public void Options()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            _optionPanel.SetActive(true);
        }

        public void NoPopup()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            _WarningPanel.SetActive(false);
        }

        public void Quit()
        {
            PlaySound(_scriptablesWorldAudio, EnumWorldSounds.Sound1);
            Application.Quit();
        }

        protected void PlaySound(ScriptablesWorldAudio _audioScript, EnumWorldSounds _action)
        {
            _observableSound.NotifyObserver(_audioScript, _action);
        }
    }
}