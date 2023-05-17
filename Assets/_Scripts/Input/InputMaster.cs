using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMaster : Singleton<InputMaster>
{
    InputsActions inputAction;

    public InputsActions InputAction { get => inputAction;  }

    private void Awake()
    {
        inputAction = new InputsActions();
        inputAction.Enable();
    }
    private void OnEnable()
    {
        inputAction.Enable();
    }
    private void OnDisable()
    {
        inputAction.Disable();
    }
}
