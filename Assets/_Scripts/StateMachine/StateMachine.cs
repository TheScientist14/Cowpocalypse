using UnityEngine;

namespace _Scripts
{
    public class StateMachine : Singleton<StateMachine>
    {
        [SerializeField] State _startingState;

        State _currentState;

        void Start()
        {
            SetState(_startingState);
        }

        void Update()
        {
            if (_currentState == null)
                return;
            
            _currentState.Update();
        }

        public void SetState(State state)
        {
            if (_currentState != null)
                _currentState.Exit();
            
            _currentState = state;
            _currentState.Enter();
        }
    }
}