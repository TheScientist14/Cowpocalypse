using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Camera _camera;
        [SerializeField] RectTransform _titleRect;
        [SerializeField] TextMeshProUGUI _titleText;
        [SerializeField] RectTransform _buttonsRect;
        [SerializeField] CanvasGroup _buttons;
        [SerializeField] Button _newGameButton;
        [SerializeField] Button _loadButton;
        [SerializeField] Button _optionsButton;
        [SerializeField] Button _quitButton;

        void Start()
        {
            _camera.orthographicSize = 2;
            _titleRect.anchoredPosition = new Vector2(0, 680);
            _buttons.alpha = 0;
            _buttonsRect.anchoredPosition = new Vector2(-300, 0);
            
            var sequence = DOTween.Sequence();
            sequence.Append(_camera.DOOrthoSize(5, 2));
            sequence.Append(_titleRect.DOAnchorPosY(280, 0.25f).SetEase(Ease.OutBack));
            sequence.Append(_camera.DOShakePosition(2f));
            sequence.Append(_buttons.DOFade(1, 2));
            sequence.Insert(3, _buttonsRect.DOAnchorPosX(0, 2));
        }

        public void NewGame()
        {
            Debug.Log("New game !");
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
            Debug.Log("Quit !");
        }
    }
}