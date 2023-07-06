using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMachinePanelInput : MonoBehaviour
{
    [SerializeField] MachinePanel m_MachinePanel;

    InputsActions m_InputsActions;
    Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        m_InputsActions = InputMaster.instance.InputAction;
        m_InputsActions.Player.LongPressButton.performed += _ => OpenMachinePanel();
    }

    private void OpenMachinePanel()
    {
        Vector2 mousePosition = m_InputsActions.Player.PointerPosition.ReadValue<Vector2>();
        Vector3 worldMousePos = m_Camera.ScreenToWorldPoint(mousePosition);
        IItemHandler itemHandler = GridManager.instance.GetItemHandlerAt(worldMousePos);

        if(!(itemHandler is Machine))
            return;

        m_MachinePanel.SetMachine(itemHandler as Machine);
        m_MachinePanel.Open();
    }
}
