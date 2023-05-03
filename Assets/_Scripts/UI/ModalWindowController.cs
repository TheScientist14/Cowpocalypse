using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowController : MonoBehaviour
{
    [SerializeField]
    private TextUI _windowName;
    [SerializeField, Header("MachineSettingsPanel")]
    private RectTransform _machineSettingsPanel;
    [SerializeField, Header("RecipeUnlockPanel")]
    private RectTransform _recipeUnlockPanel;
    private void EnableOnlyNChilds(int n, Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i < n);
        }
    }


    [Button("Switch")]
    public void SwitchPanel()
    {
        if (_machineSettingsPanel.gameObject.activeSelf)
        {
            _machineSettingsPanel.gameObject.SetActive(false);
            _recipeUnlockPanel.gameObject.SetActive(true);
        }
        else
        {
            _machineSettingsPanel.gameObject.SetActive(true);
            _recipeUnlockPanel.gameObject.SetActive(false);
        }
    }
}
