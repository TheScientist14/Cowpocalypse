using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebinder : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_ActionNameText;
    [SerializeField] TextMeshProUGUI m_BoundKeyText;
    [SerializeField] Button m_RebindButton;
    [SerializeField] Button m_RemoveBindindButton;

    private InputAction m_Action;
    private int m_BindingIndex;

    void Start()
    {
        m_RebindButton.onClick.AddListener(Rebind);
        m_RemoveBindindButton.onClick.AddListener(RemoveBindings);
    }

    public void SetInputAction(InputAction iInputAction, int iBindingIndex = 0)
    {
        m_Action = iInputAction;
        m_BindingIndex = iBindingIndex;

        if(m_Action == null)
            return;

        m_ActionNameText.text = m_Action.name;
        if(m_Action.bindings[m_BindingIndex].isPartOfComposite)
            m_ActionNameText.text += "/" + m_Action.bindings[m_BindingIndex].name;

        Refresh();
    }

    void Refresh()
    {
        if(m_Action == null || m_Action.bindings[m_BindingIndex] == null)
        {
            m_BoundKeyText.text = "";
            return;
        }

        InputBinding inputBinding = m_Action.bindings[m_BindingIndex];
        if(inputBinding.isPartOfComposite)
        {
            m_BoundKeyText.text = InputControlPath.ToHumanReadableString(
                inputBinding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
            m_BoundKeyText.text = m_Action.GetBindingDisplayString(m_BindingIndex);

        /*string inputNamesList = "";
        foreach(InputBinding inputBinding in m_Action.bindings)
        {
            if(inputNamesList != "")
                inputNamesList += ";";
            inputNamesList += m_Action.GetBindingDisplayString(inputBinding);
        }
        m_BoundKeyText.text = inputNamesList;*/
    }

    void Rebind()
    {
        if(m_Action == null)
        {
            Debug.LogError("No action...");
            return;
        }

        InputActionRebindingExtensions.RebindingOperation rebindOperation = m_Action.PerformInteractiveRebinding(m_BindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            // .WithExpectedControlType("Keyboard")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
                {
                    Debug.Log("Canceled rebinding");
                    operation.Dispose();
                })
            .OnComplete(operation =>
                {
                    Debug.Log($"Rebound '{m_Action}' to '{operation.selectedControl}'");
                    operation.Dispose();
                    Refresh();
                })
            .Start();
        Debug.Log($"Started rebinding '{m_Action}'");
    }

    void RemoveBindings()
    {
        if(m_Action == null)
            return;

        m_Action.ApplyBindingOverride(m_BindingIndex, "");
        Refresh();
    }
}
