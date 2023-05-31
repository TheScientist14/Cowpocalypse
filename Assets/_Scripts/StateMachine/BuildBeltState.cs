﻿public class BuildBeltState : State
{
    InputsActions _inputAction;
    
    public override void Enter()
    {
        base.Enter();
        
        _inputAction = InputMaster.instance.InputAction;
        
        _inputAction.Player.DragBuildMode.Enable();
        _inputAction.Player.ClickButton.Disable();
        _inputAction.Player.Drag.Disable();
    }

    public override void Exit()
    {
        base.Exit();
        
        _inputAction.Player.DragBuildMode.Disable();
        _inputAction.Player.ClickButton.Enable();
        _inputAction.Player.Drag.Enable();
    }
}