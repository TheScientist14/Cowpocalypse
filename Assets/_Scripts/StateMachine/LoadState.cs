using UnityEngine;

namespace _Scripts
{
    public class LoadState : State
    {
        public override void Enter()
        {
            InputMaster.instance.InputAction.Disable();
            Time.timeScale = 0;
        }

        public override void Exit()
        {
            InputMaster.instance.InputAction.Enable();
            Time.timeScale = 1;
        }
    }
}