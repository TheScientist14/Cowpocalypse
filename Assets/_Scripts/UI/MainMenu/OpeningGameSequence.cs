using DG.Tweening;
using NaughtyAttributes;
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

		m_IntroSequence = DOTween.Sequence();
		m_IntroSequence.Append(_camera.DOOrthoSize(5, 2));
		m_IntroSequence.Append(_titleTransform.DOMoveY(3, 0.25f).SetEase(Ease.OutBack));
		m_IntroSequence.Append(_camera.DOShakePosition(1f, 1f));
		m_IntroSequence.Append(_buttons.DOFade(1, 2).SetEase(Ease.Linear));
		m_IntroSequence.Insert(3, _buttonsRect.DOAnchorPosX(0, 2));
		m_IntroSequence.onComplete = () => SkipSequence();
	}

	void Update()
	{
		if(Input.anyKeyDown)
			SkipSequence();
	}

	[Button]
	public void SkipSequence()
	{
		if(m_IntroSequence != null && m_IntroSequence.active)
		{
			Sequence sequence = m_IntroSequence;
			m_IntroSequence = null;
			sequence.Complete(false);
		}

		_buttons.interactable = true;
		Destroy(this);
	}
}
