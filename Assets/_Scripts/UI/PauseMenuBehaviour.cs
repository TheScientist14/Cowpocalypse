using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenuBehaviour : MonoBehaviour
{
    [SerializeField] ConfirmBehaviour m_ConfirmationWidget;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ClosePauseMenu()
    {
        // #TODO : Resume game
        Debug.LogWarning("TODO: Resume game");
        gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        // #TODO : Save game
        Debug.LogWarning("TODO: Save game");
    }

    public void LoadGame()
    {
        // #TODO : Load game
        Debug.LogWarning("TODO: Load game");
    }

    public void ShowSettings()
    {
        // #TODO : Show settings
        Debug.LogWarning("TODO: Show settings");
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
            _Quit();
            return;
        }

        m_ConfirmationWidget.AskForConfirmation("Do you really want to quit? Unsaved progress will be lost.", _Quit);
    }
}
