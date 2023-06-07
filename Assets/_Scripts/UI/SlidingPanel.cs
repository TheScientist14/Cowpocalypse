using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.RectTransform;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlidingPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rtTomove;
    [SerializeField]
    private Axis _axis;
    [SerializeField, Range(.1f, 2f)]
    private float _duration = .5f;
    [SerializeField]
    private Ease _ease;
    private bool _expanded;
    private List<Tween> _runningTweens = new List<Tween>(2);

    [SerializeField] Button m_ExpandButton;
    [SerializeField] Button m_CollapseButton;

    private void Awake()
    {
        m_ExpandButton?.onClick.AddListener(Expand);
        m_CollapseButton?.onClick.AddListener(Collapse);
        Collapse();
    }

    [ContextMenu("Collaspe")]
    public void Collapse()
    {
        Toggle(false);
        m_ExpandButton.gameObject.SetActive(true);
        m_CollapseButton.gameObject.SetActive(false);
    }

    [ContextMenu("Expand")]
    public void Expand()
    {
        Toggle(true);
        m_ExpandButton.gameObject.SetActive(false);
        m_CollapseButton.gameObject.SetActive(true);
    }
    public void Toggle(bool expand)
    {
        if(_expanded == (_expanded = expand))
            return;
        _runningTweens.ForEach(t => t.Kill());
        _runningTweens.Clear();
        var targetMin = _rtTomove.anchorMin;
        var targetMax = _rtTomove.anchorMax;
        if(_axis == Axis.Vertical)
        {
            targetMin.x = _expanded ? 0 : -1;
            targetMax.x = _expanded ? 1 : 0;
        }
        if(_axis == Axis.Horizontal)
        {
            targetMin.y = _expanded ? 0 : -1;
            targetMax.y = _expanded ? 1 : 0;
        }
        _runningTweens.Add(_rtTomove.DOAnchorMin(targetMin, _duration).SetEase(_ease));
        _runningTweens.Add(_rtTomove.DOAnchorMax(targetMax, _duration).SetEase(_ease));
    }
}