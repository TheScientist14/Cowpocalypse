using NaughtyAttributes;
using NaughtyAttributes.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowController : Singleton<ModalWindowController>
{
    [SerializeField]
    private MachineSettingsPanel _machineSettingsPanel;
    [SerializeField]
    private AllTiersPanel _recipeUnlockPanel;
    [SerializeField]
    private TitlePanel _titlePanel;
    [SerializeField] private ButtonPanel _closeButton;
    [SerializeField] private ButtonPanel _returnButton;
    private Stack<Panel> _openedPanels = new Stack<Panel>();
    public bool _inCatalog => _recipeUnlockPanel.CurrentlyOpened;
    private bool InMachineSettings => _machineSettingsPanel.CurrentlyOpened;
    private void Awake()
    {
        _returnButton.Button.onClick.AddListener(CloseLast);
        _closeButton.Button.onClick.AddListener(CloseAll);
    }
    private void Start()
    {
        _recipeUnlockPanel.ChangeVisibility(false, 0f, 0f);
        _machineSettingsPanel.ChangeVisibility(false, 0f, 0f);
        _titlePanel.ChangeVisibility(false, 0f, 0f);
    }
    #region CalledFromUi
    public void OpenCatalogFromGame() => OpenCatalog("Recipe unlocks");

    public void OpenCatalogFromMachineSettings() => OpenCatalog("Recipe to craft", 1);
    private void OpenCatalog(string title, int tiersToSkip = 0)
    {
        _recipeUnlockPanel.ChangeTiersDisplayed(tiersToSkip);
        OpenPanel(_recipeUnlockPanel, title);
    }
    public void CloseCatalog()
    {
        ClosePanel(_recipeUnlockPanel);
    }
    public void OpenMachineSettings(Machine machine = null)
    {
        OpenPanel(_machineSettingsPanel, "Machine settings");
        _machineSettingsPanel.OpenedMachine = machine;
        Debug.LogWarning("Integrate with machine settings data stored to show the display accordinginly to selected machine");
    }
    public void CloseMachineSettings()
    {
        ClosePanel(_machineSettingsPanel);
    }
    private void OpenPanel(Panel panel, string title)
    {
        _openedPanels.Push(panel);
        panel.ChangeVisibility(true);
        _titlePanel.Title.Value = title;
        _titlePanel.ChangeVisibility(true);
        UpdateUiWithStackCount();
    }
    private void ClosePanel(Panel panel)
    {
        if (_openedPanels.Peek() == panel)
        {
            ClosePanelInStack(_openedPanels.Pop());
        }
        else
        {
            var removed = new List<Panel>(_openedPanels);
            removed.Remove(panel);
            _openedPanels = new Stack<Panel>(removed);
            ClosePanelInStack(panel);
        }
    }
    public void CloseAll()
    {
        while (_openedPanels.Count > 0)
            ClosePanelInStack(_openedPanels.Pop());
    }
    public void CloseLast()
    {
        ClosePanelInStack(_openedPanels.Pop());
    }
    void ClosePanelInStack(Panel panel)
    {
        panel.ChangeVisibility(false);
        UpdateUiWithStackCount();
    }

    private void UpdateUiWithStackCount()
    {
        switch (_openedPanels.Count)
        {
            case 0: CloseEverything(); break;
            case 1: _returnButton.ChangeVisibility(false); break;
            default: _returnButton.ChangeVisibility(true); break;
        }
    }

    private void CloseEverything()
    {
        _titlePanel.ChangeVisibility(false);
    }
    #endregion
    internal void RessourceClicked(RessourceUI ressourceUI)
    {
        if (_inCatalog)
        {
            if (InMachineSettings)
            {
                Debug.LogWarning("Integrate with machine production logic using stored itemData");
                _machineSettingsPanel.SetItemData(ressourceUI.ItemData);
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
    /*    [Button("Switch")]
        public void SwitchPanel()
        {
            ShowPanel(!_machineSettingsPanel.gameObject.activeSelf);
        }
        public void ShowPanel(bool showMachineSettings)
        {
            _machineSettingsPanel.ChangeVisibility(showMachineSettings);
            _recipeUnlockPanel.ChangeVisibility(!showMachineSettings);
        }*/
}
