using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SlidingPanel : PanelComponent
{
    [SerializeField] RectTransform m_RectTransformToMove;
    [SerializeField] RectTransform.Axis m_MovementAxis;
    [SerializeField, Range(.1f, 2f)] float m_MovementDuration = .5f;
    [SerializeField] Ease m_MovementEase;

    private bool m_IsExpanded;
    private List<Tween> m_RunningTweens = new List<Tween>(2);

    [SerializeField] Button m_ExpandButton;
    [SerializeField] Button m_CollapseButton;

    private void Awake()
    {
        m_ExpandButton.onClick.AddListener(Open);
        m_CollapseButton.onClick.AddListener(Close);
        Close();
    }

    protected override void OnOpen()
    {
        m_ExpandButton.gameObject.SetActive(false);
        m_CollapseButton.gameObject.SetActive(true);
        Toggle(true);
    }

    protected override void OnClose()
    {
        m_ExpandButton.gameObject.SetActive(true);
        m_CollapseButton.gameObject.SetActive(false);
        Toggle(false);
    }

    private void Toggle(bool iExpand)
    {
        if(m_IsExpanded == iExpand)
            return;
        m_IsExpanded = iExpand;

        m_RunningTweens.ForEach(t => t.Kill());
        m_RunningTweens.Clear();

        var targetMin = m_RectTransformToMove.anchorMin;
        var targetMax = m_RectTransformToMove.anchorMax;
        if(m_MovementAxis == RectTransform.Axis.Horizontal)
        {
            targetMin.x = m_IsExpanded ? 0 : -1;
            targetMax.x = m_IsExpanded ? 1 : 0;
        }
        if(m_MovementAxis == RectTransform.Axis.Vertical)
        {
            targetMin.y = m_IsExpanded ? 0 : -1;
            targetMax.y = m_IsExpanded ? 1 : 0;
        }
        m_RunningTweens.Add(m_RectTransformToMove.DOAnchorMin(targetMin, m_MovementDuration).SetEase(m_MovementEase).SetUpdate(true));
        m_RunningTweens.Add(m_RectTransformToMove.DOAnchorMax(targetMax, m_MovementDuration).SetEase(m_MovementEase).SetUpdate(true));
    }
}