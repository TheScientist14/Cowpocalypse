using UnityEngine;

namespace _Scripts
{
    public class PauseState : State
    {
        public override void Enter()
        {
            base.Enter();

            Time.timeScale = 0;
        }

        public override void Exit()
        {
            base.Exit();

            Time.timeScale = 1;
        }
    }
}