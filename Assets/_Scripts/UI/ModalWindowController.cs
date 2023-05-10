using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowController : Singleton<ModalWindowController>
{
    [SerializeField]
    private TextUI _windowName;
    [SerializeField, Header("MachineSettingsPanel")]
    private MachineSettingsPanel _machineSettingsPanel;
    [SerializeField, Header("RecipeUnlockPanel")]
    private Panel _recipeUnlockPanel;
    
    public bool _inCatalog => _recipeUnlockPanel.CurrentlyOpened;
    private bool InMachineSettings=>_machineSettingsPanel.CurrentlyOpened;

    private void Start()
    {
        _recipeUnlockPanel.ChangeVisibility(false, 0f, 0f);
        _machineSettingsPanel.ChangeVisibility(false, 0f, 0f);
    }
    #region CalledFromUi
    public void OpenCatalogFromGame()
    {/*
        _inMachineSettings = false;*/
        OpenCatalog();
    }
    public void OpenCatalogFromMachineSettings()
    {
        OpenCatalog();
    }

    private void OpenCatalog()
    {
        /*_inCatalog = true;*/
        _recipeUnlockPanel.ChangeVisibility(true);
    }

    public void CloseCatalog()
    {
        /*_inCatalog = false;*/
        _recipeUnlockPanel.ChangeVisibility(false);
    }
    public void OpenMachineSettings()
    {
        /*_inMachineSettings = true;*/
        _machineSettingsPanel.ChangeVisibility(true);
    }
    public void CloseMachineSettings()
    {
        /*_inMachineSettings = false;*/
        _machineSettingsPanel.ChangeVisibility(false);
    }
    public void CloseAll()
    {
        /*_inMachineSettings = false;*/
        CloseMachineSettings();
        CloseCatalog();
    }
    public void CloseLast()
    {
        if (_inCatalog)
            CloseCatalog();
        else if (InMachineSettings)
            CloseMachineSettings();
    }
    #endregion
    internal void RessourceClicked(RessourceUI ressourceUI)
    {
        if (_inCatalog)
        {
            if (InMachineSettings)
            {
                Debug.LogWarning("Integrate with machine production logic using stored itemData");
                _machineSettingsPanel.RecipeRessourceUI.ItemData = ressourceUI.ItemData;
                CloseCatalog();
            }
            else
            {
                Debug.LogWarning("Integrate unlocking logic");
            }
        }
        else
        {
            Debug.LogError("Got a RessourceClicked event, but we're not in catalog");
            //Same call made from UIEvents
            /*if (_inMachineSettings)
                OpenCatalogFromMachineSettings();   */
        }
    }
    [Button("Switch")]
    public void SwitchPanel()
    {
        ShowPanel(!_machineSettingsPanel.gameObject.activeSelf);
    }
    public void ShowPanel(bool showMachineSettings)
    {
        _machineSettingsPanel.ChangeVisibility(showMachineSettings);
        _recipeUnlockPanel.ChangeVisibility(!showMachineSettings);
    }
}
