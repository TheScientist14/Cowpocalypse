using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPanel : Panel
{
    [SerializeField] private Button _button;

    public Button Button { get => _button;  }
}
