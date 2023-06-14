using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private TextUI _requiredCount;
    [SerializeField] private string _requiredPrefix = "/";
    [SerializeField] private TextUI _currentCount;
    [SerializeField] private GameObject _currentText;
    [SerializeField] private GameObject _maxText;
    [SerializeField] private TextMeshProUGUI _unlockPrice;
    [SerializeField] private GameObject _lockedIcon;
    
    private ItemData _itemData;
    
    public void UpdateValue(int? current = null, int? max = null)
    {
        if (!ItemData.Unlocked)
        {
            _lockedIcon.SetActive(true);
            _unlockPrice.gameObject.SetActive(true);
            _unlockPrice.text = $"{ItemData.Tier.UnlockPrice}";
            
            _currentText.SetActive(false);
            _maxText.SetActive(false);
            
            _image.color = Color.black;
        }
        else
        {
            _lockedIcon.SetActive(false);
            _unlockPrice.gameObject.SetActive(false);
            
            _currentText.SetActive(true);
            _maxText.SetActive(true);
            
            _image.color = Color.white;
            
            if(max.HasValue)
            {
                if(current.HasValue)
                {
                    _currentCount.Value = current.Value.ToString();
                    _requiredCount.Value = _requiredPrefix + max.Value.ToString();
                }
                else
                {
                    _currentCount.Value = "";
                    _requiredCount.Value = max.Value.ToString();
                }
            }
            else if(current.HasValue)
            {
                _currentCount.Value = current.Value.ToString();
            }
        }
    }
    
    public ItemData ItemData
    {
        get => _itemData; 
        set
        {
            _itemData = value;
            if(_itemData == null)
            {
                _currentCount.Value = "NO";
                _requiredCount.Value = "NE";
                _image.sprite = null;
                gameObject.name = "None";
                return;
            }
            _image.sprite = _itemData.Sprite;
            gameObject.name = _itemData.name;
            UpdateValue(max: _itemData.Price);
        }
    }
    
    public void OnClick()
    {
        ModalWindowController.instance.RessourceClicked(this);
    }
}