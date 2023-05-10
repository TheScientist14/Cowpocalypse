using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] RectTransform _panel;
    [SerializeField] StatManager _statManager;
    [SerializeField] StatUI _statUIPrefab;

    void Start()
    {
        foreach (var stat in _statManager.Stats)
        {
            var statUI = Instantiate(_statUIPrefab, _panel);
            statUI.Stat = stat;
        }
    }

    [ContextMenu("Unfold")]
    public void Unfold()
    {
        _panel.DOAnchorPosY(0, 1f).SetEase(Ease.OutBack);
    }
    
    [ContextMenu("Fold")]
    public void Fold()
    {
        _panel.DOAnchorPosY(-200, 1f).SetEase(Ease.InBack);
    }
}