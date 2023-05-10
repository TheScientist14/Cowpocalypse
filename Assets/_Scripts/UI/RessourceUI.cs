using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private TextUI _numberDisplay;
    public int Number
    {
        set => _numberDisplay.Value = value.ToString();
    }
    private ItemData _itemData;

    public ItemData ItemData
    {
        get => _itemData; set
        {
            _itemData = value;
            _image.sprite = _itemData.Sprite;
            gameObject.name = _itemData.name;
            Number = _itemData.Price;
        }
    }
    public void OnClick()
    {
        ModalWindowController.instance.RessourceClicked(this);
    }
}
