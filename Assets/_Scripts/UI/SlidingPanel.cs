using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.RectTransform;
using UnityEngine.InputSystem;

public class SlidingPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rtTomove;
    [Header("At least one")]
    [SerializeField,Tooltip("The effect will be triggered over the moving Rect")]
    private bool _hoverRtToMove;
    [SerializeField,Tooltip("The effect will be triggered over the main steady rect")]
    private bool _hoveRtMain;
    private RectTransform _rtMain;
    [SerializeField]
    private Axis _axis;
    [SerializeField, Range(.1f, 2f)]
    private float _duration = .5f;
    [SerializeField]
    private Ease _ease;
    private bool _expanded;
    private List<Tween> _runningTweens = new List<Tween>(2);
    InputsActions a;
    private void Awake()
    {
        a = InputMaster.instance.InputAction;
        _rtMain = transform as RectTransform;
    }
    private void OnEnable()
    {
        a.Player.PointerPosition.performed += ctx => CheckValue(ctx.ReadValue<Vector2>());
    }
    private void OnDisable()
    {
        a.Player.PointerPosition.performed -= ctx => CheckValue(ctx.ReadValue<Vector2>());
    }
    public void CheckValue(Vector2 val)
    {
        Toggle((_hoverRtToMove && IsPointOverRect(_rtTomove, val)) || (_hoveRtMain && IsPointOverRect(_rtMain, val)));
    }
    public bool IsPointOverRect(RectTransform r, Vector2 val)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(r, val, null, out Vector2 localMousePos) && r.rect.Contains(localMousePos);
    }
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
        Toggle(false);
    }

    public void Expand()
    {
        Toggle(true);
    }
    public void Toggle(bool expand)
    {
        if (_expanded == (_expanded = expand))
            return;
        _runningTweens.ForEach(t => t.Kill());
        _runningTweens.Clear();
        var targetMin = _rtTomove.anchorMin;
        var targetMax = _rtTomove.anchorMax;
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
        _runningTweens.Add(_rtTomove.DOAnchorMin(targetMin, _duration).SetEase(_ease));
        _runningTweens.Add(_rtTomove.DOAnchorMax(targetMax, _duration).SetEase(_ease));
    }
}
