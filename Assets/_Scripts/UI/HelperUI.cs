using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TextUI
{
    [SerializeField]
    private string _value;
    [SerializeField]
    private TMP_Text _text;

    public string Value
    {
        get => _value; set
        {
            _value = value;
            UpdateText();
        }
    }
    void UpdateText()
    {
        _text.text = _value;
    }
}
