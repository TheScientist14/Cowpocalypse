using DG.Tweening;
using NaughtyAttributes;
using System;
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
    private RectTransform _machineSettingsPanel;
    [SerializeField, Header("RecipeUnlockPanel")]
    private RectTransform _recipeUnlockPanel;
    [SerializeField]
    private Button _back;
    [Header("Transition settings")]
    [SerializeField, Range(0, 10f)]
    private float _duration;
    [SerializeField]
    private Ease _ease;
    private RessourceUI _lastClickedResource;
    public bool _inMachineSettings = false;
    public bool _inCatalog = false;
    private void Start()
    {
        ChangeVisibility(_recipeUnlockPanel, false, 0f, 0f);
        ChangeVisibility(_machineSettingsPanel, false, 0f, 0f);
    }
    [Button("Switch")]
    public void SwitchPanel()
    {
        ShowPanel(!_machineSettingsPanel.gameObject.activeSelf);
    }
    #region CalledFromUi
    public void OpenCatalogFromGame()
    {
        _inMachineSettings = false;
        OpenCatalog();
    }
    public void OpenCatalogFromMachineSettings()
    {
        _back.gameObject.SetActive(true);
        OpenCatalog();
    }

    private void OpenCatalog()
    {
        _inCatalog = true;
        ChangeVisibility(_recipeUnlockPanel, true);
    }

    public void CloseCatalog()
    {
        _inCatalog = false;
        _back.gameObject.SetActive(false);
        ChangeVisibility(_recipeUnlockPanel, false);
    }
    public void OpenMachineSettings()
    {
        _inMachineSettings = true;
        ChangeVisibility(_machineSettingsPanel, true);
    }
    public void CloseMachineSettings()
    {
        _inMachineSettings = false;
        ChangeVisibility(_machineSettingsPanel, false);
    }
    public void CloseAll()
    {
        _inMachineSettings = false;
        CloseMachineSettings();
        CloseCatalog();
    }
    public void CloseLast()
    {
        if (_inCatalog)
            CloseCatalog();
        else if (_inMachineSettings)
            CloseMachineSettings();
    }
    #endregion
    internal void RessourceClicked(RessourceUI ressourceUI)
    {
        if (_inCatalog)
        {
            if (_inMachineSettings)
            {
                //SetRecipe in machine settings
                CloseCatalog();
            }
            else
            {
                //Unlock
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="showMachineSettings"></param>
    /// <param name="origin">When we trigger call from a ressourceUI we might need to pass it trought this method to operate things</param>
    public void ShowPanel(bool showMachineSettings, RessourceUI origin = null)
    {
        ChangeVisibility(_machineSettingsPanel, showMachineSettings);
        ChangeVisibility(_recipeUnlockPanel, !showMachineSettings);
        _lastClickedResource = origin;
    }

    private void ChangeVisibility(RectTransform rt, bool show, float delay = 0f, float? durationOverride = null)
    {
        if (rt.gameObject.activeSelf == show)
            return;
        var dur = durationOverride.HasValue ? durationOverride.Value : _duration;
        rt.gameObject.SetActive(true);
        rt.localScale = !show ? Vector3.one : Vector3.zero;
        rt.DOScale(show ? Vector3.one : Vector3.zero, dur)
            .SetEase(_ease)
            .OnComplete(() =>
        rt.gameObject.SetActive(show)
        ).SetDelay(delay);
    }


}
