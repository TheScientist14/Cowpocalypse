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
    public void OpenMachineSettings(Vector2 screenPosition, Machine machine)
    {

        //We might try to open a new panel when clicking on a different machine outside of the panel, if tryting to open in any other circunstances we don't refresh UI
        if (PointIsOutsideOfPanel(screenPosition) && machine != _machineSettingsPanel.OpenedMachine)
            CloseAll();
        else
            return;
        OpenPanel(_machineSettingsPanel, "Machine settings");
        _machineSettingsPanel.OpenedMachine = machine;
        Debug.LogWarning("Integrate with machine settings stocks");
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
        if (ressourceUI.ItemData.Unlocked)
        {
            Debug.Log($"{ressourceUI.ItemData.Name} already unlocked.");
            return;
        }

        if (!_inCatalog)
        {
            Debug.LogError("Got a RessourceClicked event, but we're not in catalog");
            return;
            //Same call made from UIEvents
            /*if (_inMachineSettings)
                OpenCatalogFromMachineSettings();   */
        }

        if (InMachineSettings)
        {
            Debug.LogWarning("Integrate with machine production logic using stored itemData");
            _machineSettingsPanel.SetItemData(ressourceUI.ItemData);
            ClosePanel(_recipeUnlockPanel);
            return;
        }

        if (Wallet.instance.Money < ressourceUI.ItemData.Tier.UnlockPrice)
        {
            // TODO: ajout d'un feedback (son, message ou autre) pour prévenir le joueur qu'il n'as pas assez d'argent
            Debug.LogWarning(
                $"Not enough cash to unlock {ressourceUI.ItemData.Name}. Need ${ressourceUI.ItemData.Tier.UnlockPrice - Wallet.instance.Money} more.");
            return;
        }

        Wallet.instance.Money -= ressourceUI.ItemData.Tier.UnlockPrice;
        ressourceUI.ItemData.Unlocked = true;
        Debug.Log($"Successfully unlocked {ressourceUI.ItemData.Name}.");
        //TODO: rajouter les item unlocked dans le save system
    }

    internal void CheckClickedOutside(Vector2 position)
    {
        if (PointIsOutsideOfPanel(position))
            CloseAll();
    }

    private bool PointIsOutsideOfPanel(Vector2 position)
    {
        return _openedPanels.Count == 0 || !RectTransformUtility.RectangleContainsScreenPoint((transform as RectTransform), position);
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