using UnityEngine.InputSystem;

public class FreeViewState : State
{
    InputsActions _inputAction;

    public override void Enter()
    {
        base.Enter();

        _inputAction = InputMaster.instance.InputAction;

        _inputAction.Player.DragBuildMode.Disable();
        _inputAction.Player.ClickBuildMode.Disable();
    }
}