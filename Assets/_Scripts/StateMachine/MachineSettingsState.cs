using UnityEngine.InputSystem;

public class MachineSettingsState : State
{
	InputsActions _inputAction;

	public override void Enter()
	{
		base.Enter();

		_inputAction = InputMaster.instance.InputAction;

		_inputAction.Player.Drag.Disable();
		_inputAction.Player.Pinch1.Disable();
		_inputAction.Player.Pinch2.Disable();
		_inputAction.Player.ZoomValue.Disable();

		foreach(InputAction inputAction in _inputAction)
		{
			if(inputAction == null)
				continue;

			for(int bindingIdx = 0; bindingIdx < inputAction.bindings.Count; bindingIdx++)
			{
				InputBinding inputBinding = inputAction.bindings[bindingIdx];
				if(inputBinding == null || inputBinding.groups == null)
					continue;
				if(inputBinding.groups.Contains("Rebindable"))
				{
					inputAction.Disable();
					break;
				}
			}
		}
	}

	public override void Exit()
	{
		base.Exit();

		_inputAction.Player.Drag.Enable();
		_inputAction.Player.Pinch1.Enable();
		_inputAction.Player.Pinch2.Enable();
		_inputAction.Player.ZoomValue.Enable();

		foreach(InputAction inputAction in _inputAction)
		{
			if(inputAction == null)
				continue;

			for(int bindingIdx = 0; bindingIdx < inputAction.bindings.Count; bindingIdx++)
			{
				InputBinding inputBinding = inputAction.bindings[bindingIdx];
				if(inputBinding == null || inputBinding.groups == null)
					continue;
				if(inputBinding.groups.Contains("Rebindable"))
				{
					inputAction.Enable();
					break;
				}
			}
		}
	}
}
