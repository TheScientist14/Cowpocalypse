public class InputStateMachine : Singleton<InputStateMachine>
{
	private State m_CurrentState;

	void Start()
	{
		SetState(new FreeViewState());
	}

	void Update()
	{
		m_CurrentState?.Update();
	}

	public void SetState(State state)
	{
		m_CurrentState?.Exit();
		m_CurrentState = state;
		m_CurrentState.Enter();
	}

	public State GetState()
	{
		return m_CurrentState;
	}
}