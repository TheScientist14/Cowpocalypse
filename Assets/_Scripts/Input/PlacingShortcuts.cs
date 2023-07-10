using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlacingShortcuts : MonoBehaviour
{
	[Serializable]
	private struct ButtonShortcutBinding
	{
		public Button Button;
		public InputActionReference InputActionRef;
	}

	[SerializeField] List<ButtonShortcutBinding> m_ButtonShortcutBindings = new List<ButtonShortcutBinding>();

	// Start is called before the first frame update
	void Start()
	{
		InputsActions inputsActions = InputMaster.instance.InputAction;
		foreach(ButtonShortcutBinding buttonShortcutBinding in m_ButtonShortcutBindings)
		{
			if(buttonShortcutBinding.InputActionRef == null || inputsActions.FindAction(buttonShortcutBinding.InputActionRef.name) == null)
			{
				Debug.LogError("No input action to bind");
				continue;
			}
			if(buttonShortcutBinding.Button == null)
			{
				Debug.LogError("Null button to bind the shortcut " + buttonShortcutBinding.InputActionRef.name);
				continue;
			}

			inputsActions.FindAction(buttonShortcutBinding.InputActionRef.name).started += _ => ClickButton(buttonShortcutBinding.Button);
		}
	}

	private void ClickButton(Button iButton)
	{
		iButton.onClick.Invoke();
	}
}
