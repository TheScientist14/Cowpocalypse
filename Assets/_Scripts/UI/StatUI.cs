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
    Wallet _wallet;
        
    public Stat Stat
    {
        get => _stat;
        set
        {
            _stat = value;
            Draw();
        } 
    }

    void Start()
    {
        _wallet = FindObjectOfType<Wallet>();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Buy();
    }

    void Draw()
    {
        _priceText.text = Stat.IsMaxedOut ? "MAX" : $"$ {Stat.StatData.Prices[Stat.CurrentLevel - 1]}";
        _statIcon.sprite = Stat.StatData.Icon;
        _lvlImage.fillAmount = (float) (Stat.CurrentLevel - 1) / Stat.StatData.MaxLevelInclusive;
    }

    void Buy()
    {
        var price = Stat.StatData.Prices[Stat.CurrentLevel - 1];
        
        if (_wallet.Money < price)
            return;

        if (Stat.CurrentLevel >= Stat.StatData.MaxLevelInclusive)
            return;

        _wallet.Money -= price;
        Stat.CurrentLevel++;
        Draw();
    }
}