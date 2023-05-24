using System.Collections;
using System.Collections.Generic;
using _Scripts;
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
        StateMachine.instance.SetState(new PauseState());
        m_PauseMenuPanel.SetActive(true);
    }
}