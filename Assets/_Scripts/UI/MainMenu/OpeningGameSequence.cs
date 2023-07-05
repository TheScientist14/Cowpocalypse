using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningGameSequence : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] Transform _titleTransform;
    [SerializeField] RectTransform _buttonsRect;
    [SerializeField] CanvasGroup _buttons;

    private Sequence m_IntroSequence = null;

    // Start is called before the first frame update
    void Start()
    {
        _camera.orthographicSize = 2;
        _titleTransform.position = new Vector2(0, 7);
        _buttons.alpha = 0;
        _buttons.interactable = false;
        _buttonsRect.anchoredPosition = new Vector2(-400, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_camera.DOOrthoSize(5, 2));
        sequence.Append(_titleTransform.DOMoveY(3, 0.25f).SetEase(Ease.OutBack));
        sequence.Append(_camera.DOShakePosition(1f, 1f));
        sequence.Append(_buttons.DOFade(1, 2).SetEase(Ease.Linear));
        sequence.Insert(3, _buttonsRect.DOAnchorPosX(0, 2));
        sequence.onComplete = () => SkipSequence();
    }

    [Button]
    public void SkipSequence()
    {
        if(m_IntroSequence != null && m_IntroSequence.active)
        {
            Sequence sequence = m_IntroSequence;
            m_IntroSequence = null;
            sequence.Kill(true);
        }

        _buttons.interactable = true;
    }
}
