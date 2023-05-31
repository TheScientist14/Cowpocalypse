using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMachineState : State
{
    InputsActions _inputAction;

    public override void Enter()
    {
        base.Enter();

        _inputAction = InputMaster.instance.InputAction;

        _inputAction.Player.ClickBuildMode.Enable();
        _inputAction.Player.ClickButton.Disable();
    }

    public override void Exit()
    {
        base.Exit();

        _inputAction.Player.ClickBuildMode.Disable();
        _inputAction.Player.ClickButton.Enable();
    }
}
