using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.RectTransform;

public class SlidingPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rt;
    [SerializeField]
    private Axis _axis;
    [SerializeField, Range(.1f, 2f)]
    private float _duration = .5f;
    [SerializeField]
    private Ease _ease;
    private bool _expanded;
    private List<Tween> _runningTweens = new List<Tween>(2);
        //Demo script
    /*private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, 1f));
            if (UnityEngine.Random.Range(0, 2) == 0)
                Expand();
            else
                Collapse();
        }
    }*/
    /*/// <summary>
    /// Dummy
    /// </summary>
    private void Update()
    {
        if (FindObjectOfType<EventSystem>().currentSelectedGameObject == gameObject)
            Expand();
        else
            Collapse();
    }*/

    public void Collapse()
    {
        Toggler(false);
    }

    public void Expand()
    {
        Toggler(true);
    }
    public void Toggler(bool expand)
    {
        if (_expanded == (_expanded = expand))
            return;
        _runningTweens.ForEach(t => t.Kill());
        _runningTweens.Clear();
        var targetMin = _rt.anchorMin;
        var targetMax = _rt.anchorMax;
        if (_axis == Axis.Vertical)
        {
            targetMin.x = _expanded ? 0 : -1;
            targetMax.x = _expanded ? 1 : 0;
        }
        if (_axis == Axis.Horizontal)
        {
            targetMin.y = _expanded ? 0 : -1;
            targetMax.y = _expanded ? 1 : 0;
        }
        _runningTweens.Add(_rt.DOAnchorMin(targetMin, _duration).SetEase(_ease));
        _runningTweens.Add(_rt.DOAnchorMax(targetMax, _duration).SetEase(_ease));
    }
}
