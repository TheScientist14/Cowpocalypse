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
        [SerializeField] GameObject _panel;

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
            _panel.SetActive(true);
        }
        
        public void NewGame_Yes()
        {
            SceneManager.LoadScene(1);
        }

        public void NewGame_No()
        {
            _panel.SetActive(false);
        }
        
        public void Load()
        {
            Debug.Log("Load !");
        }
        
        public void Options()
        {
            Debug.Log("Options !");
        }
        
        
        public void Quit()
        {
            Application.Quit();
        }
    }
}