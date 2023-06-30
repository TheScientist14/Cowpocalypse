using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputMaster : Singleton<InputMaster>
{
    InputsActions inputAction;

    public InputsActions InputAction { get => inputAction; }

    protected new void Awake()
    {
        base.Awake();

        inputAction = new InputsActions();
        inputAction.Enable();
        SetupCallbacks();
    }

    private void OnEnable()
    {
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    ////////////////////////
    // Filtering with ui
    ////////////////////////

    public UnityEvent OnStartClickButton;
    public UnityEvent OnStartDoubleClickButton;
    public UnityEvent OnStartDrag;
    public UnityEvent OnStartDragBuildMode;
    public UnityEvent OnStartClickBuildMode;

    public void SetupCallbacks()
    {
        inputAction.Player.ClickButton.started += _ => FilterStartedClickButton();
        inputAction.Player.DoubleClickButton.started += _ => FilterStartedDoubleClickButton();
        inputAction.Player.Drag.started += _ => FilterStartedDrag();
        inputAction.Player.DragBuildMode.started += _ => FilterStartedDragBuildMode();
        inputAction.Player.ClickBuildMode.started += _ => FilterStartedClickBuildMode();
    }

    private bool IsOnUI()
    {
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
