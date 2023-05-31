using UnityEngine;

public class StateMachine : Singleton<StateMachine>
{
    State _currentState;

    void Start()
    {
        SetState(new FreeViewState());
    }

    void Update()
    {
        _currentState?.Update();
    }

    public void SetState(State state)
    {
        // Debug.Log("Current state : " + state.GetType().Name);
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }
}