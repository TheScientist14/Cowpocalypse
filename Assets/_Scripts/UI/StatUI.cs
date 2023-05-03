using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] Image _statIcon;
    [SerializeField] Image _lvlImage;
        
    Stat _stat;
        
    public Stat Stat
    {
        get => _stat;
        set
        {
            _stat = value;
            Draw();
        } 
    }
        
    void Draw()
    {
        _priceText.text = $"$ {Stat.StatData.Prices[Stat.CurrentLevel - 1]}";
        _statIcon.sprite = Stat.StatData.Icon;
        _lvlImage.fillAmount = (float) (Stat.CurrentLevel - 1) / Stat.StatData.MaxLevelInclusive;
    }
        
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Buy();
    }

    void Buy()
    {
        if (int.MaxValue < Stat.StatData.Prices[Stat.CurrentLevel - 1])
            return;

        if (Stat.CurrentLevel >= Stat.StatData.MaxLevelInclusive)
            return;

        Stat.CurrentLevel++;
        Draw();
    }
}