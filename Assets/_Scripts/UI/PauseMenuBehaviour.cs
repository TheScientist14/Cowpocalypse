using _Scripts.Save_System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenuBehaviour : MonoBehaviour
{
    [SerializeField] ConfirmBehaviour m_ConfirmationWidget;
    [SerializeField] GameObject m_OptionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        m_OptionsPanel.SetActive(false);
    }

    void OnEnable()
    {
        m_OptionsPanel.SetActive(false);
    }

    public void ClosePauseMenu()
    {
        // #TODO : Resume game
        Debug.LogWarning("TODO: Resume game");
        gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        Debug.Log("Saving...");
        SaveSystem.instance.SaveGameAsync().Wait();
    }

    public void LoadGame()
    {
        // #TODO : Load game
        Debug.LogWarning("TODO: Load game");
    }

    // we assume settings panel is covering the pause menu completely
    public void ShowSettings()
    {
        if(m_OptionsPanel == null)
        {
            Debug.LogWarning("Settings panel unbound to pause menu panel");
            return;
        }

        m_OptionsPanel.gameObject.SetActive(true);
    }

    private void _Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Quit()
    {
        if(m_ConfirmationWidget == null)
        {
            Debug.LogWarning("No confirmation panel, skipping user confirmation.");
            _Quit();
            return;
        }

        m_ConfirmationWidget.AskForConfirmation("Do you really want to quit? Unsaved progress will be lost.", _Quit);
    }
}
