using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButtonBehaviour : MonoBehaviour
{
    [SerializeField] GameObject m_PauseMenuPanel;

    void Start()
    {
        m_PauseMenuPanel.SetActive(false);
    }

    public void PauseGame()
    {
        // #TODO : Pause game
        Debug.LogWarning("TODO: Resume game");
        m_PauseMenuPanel.SetActive(true);
    }
}
