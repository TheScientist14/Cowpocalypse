using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputMaster : Singleton<InputMaster>
{
	private InputsActions inputAction;

	public InputsActions InputAction
	{
		get => inputAction;
	}

	private static string s_InputBindingPlayerPrefKey = "inputBindings";

	protected override bool IsPersistent()
	{
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		if(instance != this)
			return;

		inputAction = new InputsActions();
		string overrideJson = PlayerPrefs.GetString(s_InputBindingPlayerPrefKey, "");
		inputAction.LoadBindingOverridesFromJson(overrideJson);

		inputAction.Enable();
		SetupCallbacks();
	}

	private void OnEnable()
	{
		inputAction.Enable();
	}

	private void OnDisable()
	{
		if(inputAction == null)
		{
			Debug.LogWarning("Disabling while no input action");
			return;
		}

		inputAction.Disable();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if(inputAction == null)
			return;

		string overrideJson = inputAction.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString(s_InputBindingPlayerPrefKey, overrideJson);
	}

	////////////////////////
	// Filtering with ui
	////////////////////////

	[HideInInspector] public UnityEvent OnStartClickButton;
	[HideInInspector] public UnityEvent OnStartDoubleClickButton;
	[HideInInspector] public UnityEvent OnStartDrag;
	[HideInInspector] public UnityEvent OnStartDragBuildMode;
	[HideInInspector] public UnityEvent OnStartClickBuildMode;

	public void SetupCallbacks()
	{
		inputAction.Player.ClickButton.started += _ => FilterStartedClickButton();
		inputAction.Player.DoubleClickButton.started += _ => FilterStartedDoubleClickButton();
		inputAction.Player.Drag.started += _ => FilterStartedDrag();
		inputAction.Player.DragBuildMode.started += _ => FilterStartedDragBuildMode();
		inputAction.Player.ClickBuildMode.started += _ => FilterStartedClickBuildMode();
	}

	public void RemoveCallbacks()
	{
		inputAction.Player.ClickButton.started -= _ => FilterStartedClickButton();
		inputAction.Player.DoubleClickButton.started -= _ => FilterStartedDoubleClickButton();
		inputAction.Player.Drag.started -= _ => FilterStartedDrag();
		inputAction.Player.DragBuildMode.started -= _ => FilterStartedDragBuildMode();
		inputAction.Player.ClickBuildMode.started -= _ => FilterStartedClickBuildMode();
	}

	private bool IsOnUI()
	{
		if(EventSystem.current == null)
			return false;
		return EventSystem.current.IsPointerOverGameObject();
	}

	IEnumerator DelayInvokeIfNotOnUI(UnityEvent iEvent)
	{
		yield return new WaitForEndOfFrame();
		if(!IsOnUI())
			iEvent.Invoke();
	}

	void FilterStartedClickButton()
	{
		StartCoroutine(DelayInvokeIfNotOnUI(OnStartClickButton));
	}

	void FilterStartedDoubleClickButton()
	{
		StartCoroutine(DelayInvokeIfNotOnUI(OnStartDoubleClickButton));
	}

	void FilterStartedDrag()
	{
		StartCoroutine(DelayInvokeIfNotOnUI(OnStartDrag));
	}

	void FilterStartedDragBuildMode()
	{
		StartCoroutine(DelayInvokeIfNotOnUI(OnStartDragBuildMode));
	}

	void FilterStartedClickBuildMode()
	{
		StartCoroutine(DelayInvokeIfNotOnUI(OnStartClickBuildMode));
	}
}
