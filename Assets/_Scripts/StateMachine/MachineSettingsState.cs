using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSettingsState : State
{
    InputsActions _inputAction;

    public override void Enter()
    {
        base.Enter();

        _inputAction = InputMaster.instance.InputAction;

        _inputAction.Player.Drag.Disable();
        _inputAction.Player.ClickButton.Disable();
    }

    public override void Exit()
    {
        base.Exit();

        _inputAction.Player.Drag.Enable();
        _inputAction.Player.ClickButton.Enable();
    }
}
