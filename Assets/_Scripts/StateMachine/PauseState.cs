using UnityEngine;

public class PauseState : State
{
    public override void Enter()
    {
        base.Enter();

        InputMaster.instance.InputAction.Disable();
        Time.timeScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        InputMaster.instance.InputAction.Enable();
        Time.timeScale = 1;
    }
}