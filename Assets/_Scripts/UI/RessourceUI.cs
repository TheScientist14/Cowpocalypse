using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessourceUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private bool _switchesWindowOnClick = false;
    private ItemData _itemData;

    public ItemData ItemData
    {
        get => _itemData; set
        {
            _itemData = value;
            _image.sprite = _itemData.Sprite;
        }
    }
    public void OnClick()
    {
        ModalWindowController.instance.RessourceClicked(this);
    }
}
