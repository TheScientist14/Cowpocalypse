using UnityEngine;
using UnityEngine.UI;

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private TextUI _requiredCount;
    [SerializeField] private string _requiredPrefix = "/";
    [SerializeField] private TextUI _currentCount;
    private ItemData _itemData;
    public void UpdateValue(int? current = null, int? max = null)
    {
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
    public ItemData ItemData
    {
        get => _itemData; set
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