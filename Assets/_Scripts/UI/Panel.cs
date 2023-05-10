using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [SerializeField]
    private Button _OptionalCloseButton;
    [Header("Transition settings")]
    [SerializeField, Range(0, 10f)]
    private float _duration;
    [SerializeField]
    private Ease _ease;
    private RectTransform rt;
    private bool _currentlyOpened = false;

    public bool CurrentlyOpened { get => _currentlyOpened; }

    private void Awake()
    {
        rt = transform as RectTransform;
        if (_OptionalCloseButton != null)
            _OptionalCloseButton.onClick.AddListener(() => ChangeVisibility(false));
    }
    public void ChangeVisibility(bool show, float delay = 0f, float? durationOverride = null)
    {
        if (rt.gameObject.activeSelf == show)
            return;
        var dur = durationOverride.HasValue ? durationOverride.Value : _duration;
        rt.gameObject.SetActive(true);
        rt.localScale = !show ? Vector3.one : Vector3.zero;
        rt.DOScale(show ? Vector3.one : Vector3.zero, dur)
            .SetEase(_ease)
            .OnComplete(() =>
        rt.gameObject.SetActive(show)
        ).SetDelay(delay);
        if (_OptionalCloseButton != null)
            _OptionalCloseButton.gameObject.SetActive(show);
        _currentlyOpened = show;
    }
}
